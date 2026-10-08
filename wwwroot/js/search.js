// Patient search page.
// 1. Reads the form.  2. Checks the values.  3. Calls GET /api/patients/search.
// 4. Builds the results table from the JSON that comes back.
(function () {
    const form = document.getElementById('searchForm');
    if (!form) return;

    const errorsBox = document.getElementById('searchErrors');
    const clearButton = document.getElementById('clearButton');
    const loading = document.getElementById('resultsLoading');
    const table = document.getElementById('resultsTable');
    const tableBody = document.getElementById('resultsBody');
    const emptyState = document.getElementById('emptyState');
    const summary = document.getElementById('resultsSummary');

    const namePattern = /^[A-Za-z' -]+$/;
    const mrnPattern = /^[A-Za-z0-9-]+$/;

    form.addEventListener('submit', function (event) {
        event.preventDefault();          // stop the normal page reload
        runSearch();
    });

    clearButton.addEventListener('click', function () {
        form.reset();
        hideErrors();
        showResults(null);
        summary.textContent = 'Enter a search above to see patients.';
        history.replaceState(null, '', window.location.pathname);
        form.mrn.focus();
    });

    // If the address bar already has a search (for example after pressing Back), run it again.
    const startParams = new URLSearchParams(window.location.search);
    if (startParams.toString()) {
        form.mrn.value = startParams.get('mrn') || '';
        form.dob.value = startParams.get('dob') || '';
        form.firstName.value = startParams.get('firstName') || '';
        form.lastName.value = startParams.get('lastName') || '';
        runSearch();
    }

    function readForm() {
        return {
            mrn: form.mrn.value.trim(),
            dob: form.dob.value,                       // already "yyyy-mm-dd" from the date picker
            firstName: form.firstName.value.trim(),
            lastName: form.lastName.value.trim()
        };
    }

    // Same rules as the server checks. Checking here first gives the user instant feedback.
    function validate(values) {
        const errors = [];
        if (!values.mrn && !values.dob && !values.firstName && !values.lastName) {
            errors.push('Enter at least one search value: MRN, date of birth, first name or last name.');
        }
        if (values.mrn && !mrnPattern.test(values.mrn)) {
            errors.push('MRN can contain only letters, numbers and dashes.');
        }
        if (values.dob) {
            if (values.dob > todayAsText()) errors.push('Date of birth cannot be in the future.');
            if (values.dob < '1900-01-01') errors.push('Date of birth must be after 1900.');
        }
        if (values.firstName && !namePattern.test(values.firstName)) {
            errors.push('First name can contain only letters, spaces, hyphens and apostrophes.');
        }
        if (values.lastName && !namePattern.test(values.lastName)) {
            errors.push('Last name can contain only letters, spaces, hyphens and apostrophes.');
        }
        return errors;
    }

    async function runSearch() {
        const values = readForm();
        const errors = validate(values);
        if (errors.length > 0) {
            showErrors(errors);
            return;
        }
        hideErrors();

        // Only send the fields that were filled in, e.g. ?lastName=Harr
        const params = new URLSearchParams();
        for (const [key, value] of Object.entries(values)) {
            if (value) params.append(key, value);
        }
        history.replaceState(null, '', '?' + params.toString());   // so Back returns to these results

        setLoading(true);
        try {
            const response = await fetch('/api/patients/search?' + params.toString(), {
                headers: { 'Accept': 'application/json' }
            });

            if (response.status === 401) {
                // Session timed out: go to the login page and come back here afterwards.
                const here = window.location.pathname + window.location.search;
                window.location.href = '/Identity/Account/Login?ReturnUrl=' + encodeURIComponent(here);
                return;
            }

            if (response.status === 400) {
                // The server's validation found a problem. Show its messages.
                const problem = await response.json();
                showErrors(readProblemMessages(problem));
                showResults(null);
                return;
            }

            if (!response.ok) {
                throw new Error('Search failed with status ' + response.status);
            }

            const patients = await response.json();
            showResults(patients);
        } catch (error) {
            console.error(error);
            showErrors(['Something went wrong while searching. Please try again.']);
            showResults(null);
        } finally {
            setLoading(false);
        }
    }

    function showResults(patients) {
        tableBody.innerHTML = '';
        table.classList.add('d-none');
        emptyState.classList.add('d-none');
        if (patients === null) return;

        if (patients.length === 0) {
            emptyState.classList.remove('d-none');
            summary.textContent = 'No matches';
            return;
        }

        summary.textContent = patients.length === 1 ? '1 patient found' : patients.length + ' patients found';

        for (const p of patients) {
            const chartUrl = '/Patients/Chart/' + p.patientId;
            const row = document.createElement('tr');

            const nameCell = document.createElement('td');
            const nameLink = document.createElement('a');
            nameLink.href = chartUrl;
            nameLink.className = 'patient-link';
            nameLink.textContent = p.lastName.toUpperCase() + ', ' + p.firstName;   // textContent is safe: no HTML injection
            nameCell.appendChild(nameLink);
            row.appendChild(nameCell);

            row.appendChild(cell(p.mrn));
            row.appendChild(cell(formatDate(p.dob) + ' · ' + p.age + ' y'));
            row.appendChild(cell(p.gender));
            row.appendChild(cell(p.phoneNumber || '-'));
            row.appendChild(cell([p.city, p.state].filter(Boolean).join(', ') || '-'));

            const actionCell = document.createElement('td');
            actionCell.className = 'actions';
            const openLink = document.createElement('a');
            openLink.href = chartUrl;
            openLink.className = 'btn btn-sm btn-primary';
            openLink.textContent = 'Open chart';
            actionCell.appendChild(openLink);
            row.appendChild(actionCell);

            tableBody.appendChild(row);
        }
        table.classList.remove('d-none');
    }

    function cell(text) {
        const td = document.createElement('td');
        td.textContent = text;
        return td;
    }

    // "1958-03-14T00:00:00" -> "03/14/1958"
    function formatDate(isoText) {
        const [year, month, day] = isoText.substring(0, 10).split('-');
        return month + '/' + day + '/' + year;
    }

    function todayAsText() {
        const now = new Date();
        const month = String(now.getMonth() + 1).padStart(2, '0');
        const day = String(now.getDate()).padStart(2, '0');
        return now.getFullYear() + '-' + month + '-' + day;
    }

    // ASP.NET Core sends validation errors as { errors: { "Dob": ["message"], ... } }
    function readProblemMessages(problem) {
        if (problem && problem.errors) {
            return Object.values(problem.errors).flat();
        }
        return [(problem && problem.title) || 'The search request was not valid.'];
    }

    function showErrors(messages) {
        errorsBox.innerHTML = '';
        const list = document.createElement('ul');
        list.className = 'mb-0 ps-3';
        for (const message of messages) {
            const item = document.createElement('li');
            item.textContent = message;
            list.appendChild(item);
        }
        errorsBox.appendChild(list);
        errorsBox.classList.remove('d-none');
    }

    function hideErrors() {
        errorsBox.classList.add('d-none');
        errorsBox.innerHTML = '';
    }

    function setLoading(isLoading) {
        loading.classList.toggle('d-none', !isLoading);
        if (isLoading) {
            table.classList.add('d-none');
            emptyState.classList.add('d-none');
            summary.textContent = 'Searching...';
        }
    }
})();