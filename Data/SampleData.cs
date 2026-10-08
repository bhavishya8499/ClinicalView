using ClinicalView.Models;

namespace ClinicalView.Data;

// Made-up patients for the demo. None of this is real patient information.
// SeedData.cs saves these into the database the first time the app starts.
public static class SampleData
{
    public static List<Patient> CreatePatients()
    {
        return new List<Patient>
        {
            new Patient
            {
                MRN = "00482917", FirstName = "Margaret", LastName = "Harrison", DOB = new DateTime(1958, 3, 14),
                Gender = "Female", PhoneNumber = "(304) 555-0142", AddressLine1 = "418 Willowbrook Rd",
                City = "Morgantown", State = "WV", ZipCode = "26505",
                Allergies =
                {
                    Allergy("Penicillin", "Severe", "Active", 2019, 2, 11),
                    Allergy("Latex", "Moderate", "Active", 2021, 6, 30),
                    Allergy("Sulfa drugs", "Mild", "Active", 2023, 9, 14)
                },
                Medications =
                {
                    Medication("Lisinopril", "10 mg tablet", "Take 1 tablet by mouth daily", "Daily", "PO", 2022, 1, 5, "Dr. Anita Patel"),
                    Medication("Metformin", "500 mg tablet", "Take 1 tablet by mouth with meals", "BID", "PO", 2020, 8, 17, "Dr. Anita Patel"),
                    Medication("Atorvastatin", "40 mg tablet", "Take 1 tablet by mouth at bedtime", "QHS", "PO", 2023, 3, 2, "Dr. Lei Chen"),
                    Medication("Aspirin", "81 mg EC tablet", "Take 1 tablet by mouth daily", "Daily", "PO", 2023, 3, 2, "Dr. Lei Chen"),
                    Discontinued("Amlodipine", "5 mg tablet", "Take 1 tablet by mouth daily", "Daily", "PO", 2019, 4, 1, 2022, 1, 5, "Dr. Anita Patel")
                },
                Alerts =
                {
                    Alert("Fall Risk", "Morse fall score 55. Bed alarm on, non-slip socks.", "High", 2026, 9, 20),
                    Alert("Hearing Impaired", "Face the patient when speaking. Give written instructions.", "Medium", 2024, 5, 2)
                },
                Documents = StandardDocuments(2026, 9)
            },
            new Patient
            {
                MRN = "00519304", FirstName = "Daniel", LastName = "Harris", DOB = new DateTime(1984, 11, 2),
                Gender = "Male", PhoneNumber = "(304) 555-0187", AddressLine1 = "77 Pleasant St",
                City = "Fairmont", State = "WV", ZipCode = "26554",
                Allergies = { Allergy("Peanuts", "Severe", "Active", 2010, 5, 3) },
                Medications =
                {
                    Medication("Albuterol HFA", "90 mcg/actuation", "Inhale 2 puffs every 4 to 6 hours as needed for wheezing", "Q4-6H PRN", "Inhaled", 2018, 2, 9, "Dr. Rachel Moore"),
                    Medication("EpiPen", "0.3 mg auto-injector", "Inject into outer thigh once for severe allergic reaction", "PRN", "IM", 2010, 5, 3, "Dr. Rachel Moore")
                },
                Alerts = { Alert("Anaphylaxis Risk", "Severe peanut allergy. EpiPen at bedside.", "High", 2025, 1, 15) },
                Documents = StandardDocuments(2026, 7)
            },
            new Patient
            {
                MRN = "00377125", FirstName = "Aisha", LastName = "Harrington", DOB = new DateTime(1996, 7, 21),
                Gender = "Female", PhoneNumber = "(681) 555-0119", AddressLine1 = "12 Ridgeview Ct",
                City = "Clarksburg", State = "WV", ZipCode = "26301",
                // No allergies: the page will show "No Known Allergies".
                Medications =
                {
                    Medication("Levothyroxine", "50 mcg tablet", "Take 1 tablet by mouth every morning on an empty stomach", "Daily", "PO", 2021, 10, 12, "Dr. Lei Chen")
                },
                Documents = StandardDocuments(2026, 5)
            },
            new Patient
            {
                MRN = "00601448", FirstName = "Thomas", LastName = "Harrell", DOB = new DateTime(1949, 1, 9),
                Gender = "Male", PhoneNumber = "(304) 555-0163", AddressLine1 = "903 Mountain View Dr",
                City = "Morgantown", State = "WV", ZipCode = "26508",
                Allergies =
                {
                    Allergy("Codeine", "Moderate", "Active", 2015, 3, 22),
                    Allergy("Iodinated contrast", "Severe", "Active", 2017, 11, 8),
                    Allergy("Shellfish", "Mild", "Inactive", 2001, 6, 1)
                },
                Medications =
                {
                    Medication("Warfarin", "5 mg tablet", "Take 1 tablet by mouth daily. Check INR weekly", "Daily", "PO", 2020, 2, 14, "Dr. Samuel Brooks"),
                    Medication("Metoprolol succinate", "50 mg ER tablet", "Take 1 tablet by mouth daily", "Daily", "PO", 2020, 2, 14, "Dr. Samuel Brooks"),
                    Medication("Furosemide", "20 mg tablet", "Take 1 tablet by mouth every morning", "Daily", "PO", 2024, 8, 30, "Dr. Samuel Brooks"),
                    Discontinued("Digoxin", "0.125 mg tablet", "Take 1 tablet by mouth daily", "Daily", "PO", 2020, 2, 14, 2023, 6, 1, "Dr. Samuel Brooks")
                },
                Alerts =
                {
                    Alert("Anticoagulant", "On warfarin. High bleeding risk.", "High", 2020, 2, 14),
                    Alert("Code Status", "DNR. Do not resuscitate per advance directive.", "High", 2025, 12, 3),
                    Alert("Fall Risk", "Uses a walker. Assist with ambulation.", "Medium", 2024, 8, 30)
                },
                Documents = StandardDocuments(2026, 8)
            },
            new Patient
            {
                MRN = "00290556", FirstName = "Linh", LastName = "Nguyen", DOB = new DateTime(1972, 5, 30),
                Gender = "Female", PhoneNumber = "(304) 555-0174", AddressLine1 = "2210 University Ave",
                City = "Morgantown", State = "WV", ZipCode = "26505",
                Allergies = { Allergy("Ibuprofen", "Moderate", "Active", 2016, 7, 19) },
                Medications =
                {
                    Medication("Omeprazole", "20 mg capsule", "Take 1 capsule by mouth before breakfast", "Daily", "PO", 2022, 4, 4, "Dr. Anita Patel"),
                    Medication("Sertraline", "50 mg tablet", "Take 1 tablet by mouth daily", "Daily", "PO", 2023, 1, 23, "Dr. Maria Lopez")
                },
                Alerts = { Alert("Interpreter Needed", "Preferred language: Vietnamese.", "Medium", 2022, 4, 4) },
                Documents = StandardDocuments(2026, 6)
            },
            new Patient
            {
                MRN = "00731802", FirstName = "Chidi", LastName = "Okafor", DOB = new DateTime(1990, 12, 11),
                Gender = "Male", PhoneNumber = "(681) 555-0128", AddressLine1 = "56 Beechurst Ave",
                City = "Morgantown", State = "WV", ZipCode = "26505",
                Medications =
                {
                    Medication("Insulin glargine", "100 units/mL pen", "Inject 20 units under the skin at bedtime", "QHS", "Subcut", 2019, 9, 9, "Dr. Lei Chen"),
                    Medication("Insulin lispro", "100 units/mL pen", "Inject per sliding scale with meals", "TID AC", "Subcut", 2019, 9, 9, "Dr. Lei Chen")
                },
                Alerts = { Alert("Diabetic", "Type 1 diabetes. Check glucose before meals.", "Medium", 2019, 9, 9) },
                Documents = StandardDocuments(2026, 4)
            },
            new Patient
            {
                MRN = "00815533", FirstName = "Rosa", LastName = "Martinez", DOB = new DateTime(1965, 8, 3),
                Gender = "Female", PhoneNumber = "(304) 555-0131", AddressLine1 = "14 Oak Hollow Ln",
                City = "Bridgeport", State = "WV", ZipCode = "26330",
                Allergies =
                {
                    Allergy("Morphine", "Severe", "Active", 2018, 10, 2),
                    Allergy("Adhesive tape", "Mild", "Active", 2020, 3, 17)
                },
                Medications =
                {
                    Medication("Gabapentin", "300 mg capsule", "Take 1 capsule by mouth three times daily", "TID", "PO", 2021, 5, 11, "Dr. Maria Lopez"),
                    Medication("Losartan", "50 mg tablet", "Take 1 tablet by mouth daily", "Daily", "PO", 2019, 7, 1, "Dr. Anita Patel")
                },
                Alerts = { Alert("Isolation", "Contact precautions (MRSA history).", "High", 2026, 8, 12) },
                Documents = StandardDocuments(2026, 8)
            },
            new Patient
            {
                MRN = "00924471", FirstName = "James", LastName = "Whitfield", DOB = new DateTime(1981, 4, 17),
                Gender = "Male", PhoneNumber = "(304) 555-0156", AddressLine1 = "380 Chestnut Ridge Rd",
                City = "Morgantown", State = "WV", ZipCode = "26505",
                Allergies = { Allergy("Amoxicillin", "Moderate", "Active", 2012, 9, 26) },
                Medications =
                {
                    Medication("Montelukast", "10 mg tablet", "Take 1 tablet by mouth at bedtime", "QHS", "PO", 2017, 3, 3, "Dr. Rachel Moore"),
                    Discontinued("Prednisone", "20 mg tablet", "Take 2 tablets by mouth daily for 5 days", "Daily", "PO", 2026, 2, 1, 2026, 2, 6, "Dr. Rachel Moore")
                },
                Documents = StandardDocuments(2026, 3)
            },
            new Patient
            {
                MRN = "01033267", FirstName = "Emily", LastName = "Chen", DOB = new DateTime(2001, 2, 25),
                Gender = "Female", PhoneNumber = "(681) 555-0109", AddressLine1 = "9 Grant Ave",
                City = "Morgantown", State = "WV", ZipCode = "26505",
                Medications =
                {
                    Medication("Norgestimate/ethinyl estradiol", "0.25 mg/0.035 mg tablet", "Take 1 tablet by mouth daily", "Daily", "PO", 2022, 8, 15, "Dr. Maria Lopez")
                },
                Documents = StandardDocuments(2026, 2)
            },
            new Patient
            {
                MRN = "01147390", FirstName = "Robert", LastName = "Kowalski", DOB = new DateTime(1955, 10, 8),
                Gender = "Male", PhoneNumber = "(304) 555-0195", AddressLine1 = "1201 Smithtown Rd",
                City = "Morgantown", State = "WV", ZipCode = "26508",
                Allergies =
                {
                    Allergy("Aspirin", "Moderate", "Active", 2014, 1, 30),
                    Allergy("Bee stings", "Severe", "Active", 1998, 7, 4)
                },
                Medications =
                {
                    Medication("Tamsulosin", "0.4 mg capsule", "Take 1 capsule by mouth 30 minutes after the same meal daily", "Daily", "PO", 2021, 11, 2, "Dr. Samuel Brooks"),
                    Medication("Apixaban", "5 mg tablet", "Take 1 tablet by mouth twice daily", "BID", "PO", 2024, 2, 19, "Dr. Samuel Brooks"),
                    Medication("Tiotropium", "18 mcg inhalation capsule", "Inhale contents of 1 capsule daily", "Daily", "Inhaled", 2020, 10, 5, "Dr. Rachel Moore")
                },
                Alerts =
                {
                    Alert("Anticoagulant", "On apixaban. Hold before procedures per protocol.", "High", 2024, 2, 19),
                    Alert("Oxygen", "Home O2 2 L/min via nasal cannula.", "Medium", 2025, 3, 10)
                },
                Documents = StandardDocuments(2026, 9)
            },
            new Patient
            {
                MRN = "01250918", FirstName = "Priya", LastName = "Raman", DOB = new DateTime(1988, 6, 6),
                Gender = "Female", PhoneNumber = "(304) 555-0118", AddressLine1 = "45 Van Voorhis Rd",
                City = "Morgantown", State = "WV", ZipCode = "26505",
                Allergies = { Allergy("Erythromycin", "Mild", "Active", 2015, 4, 9) },
                Medications =
                {
                    Medication("Prenatal multivitamin", "1 tablet", "Take 1 tablet by mouth daily", "Daily", "PO", 2026, 3, 1, "Dr. Maria Lopez")
                },
                Alerts = { Alert("Pregnancy", "Second trimester. Avoid teratogenic medications.", "High", 2026, 3, 1) },
                Documents = StandardDocuments(2026, 9)
            },
            new Patient
            {
                MRN = "01368824", FirstName = "Michael", LastName = "Brennan", DOB = new DateTime(1977, 9, 19),
                Gender = "Male", PhoneNumber = "(681) 555-0177", AddressLine1 = "88 Collins Ferry Rd",
                City = "Morgantown", State = "WV", ZipCode = "26505",
                Allergies = { Allergy("Vancomycin", "Moderate", "Active", 2022, 12, 1) },
                Medications =
                {
                    Medication("Buprenorphine/naloxone", "8 mg/2 mg film", "Dissolve 1 film under the tongue daily", "Daily", "SL", 2023, 5, 15, "Dr. Maria Lopez")
                },
                Documents = StandardDocuments(2026, 1)
            }
        };
    }

    // ---- Small helper methods so the list above stays readable ----

    private static Allergy Allergy(string name, string severity, string status, int year, int month, int day) => new()
    {
        AllergyName = name,
        Severity = severity,
        Status = status,
        RecordedDate = new DateTime(year, month, day)
    };

    private static Medication Medication(string name, string strength, string instructions, string frequency,
        string route, int year, int month, int day, string provider) => new()
    {
        MedicationName = name,
        Strength = strength,
        DosageInstructions = instructions,
        Frequency = frequency,
        Route = route,
        StartDate = new DateTime(year, month, day),
        PrescribingProvider = provider,
        Status = "Active"
    };

    private static Medication Discontinued(string name, string strength, string instructions, string frequency,
        string route, int startYear, int startMonth, int startDay, int endYear, int endMonth, int endDay,
        string provider) => new()
    {
        MedicationName = name,
        Strength = strength,
        DosageInstructions = instructions,
        Frequency = frequency,
        Route = route,
        StartDate = new DateTime(startYear, startMonth, startDay),
        EndDate = new DateTime(endYear, endMonth, endDay),
        PrescribingProvider = provider,
        Status = "Discontinued"
    };

    private static PatientAlert Alert(string type, string description, string severity, int year, int month, int day) => new()
    {
        AlertType = type,
        Description = description,
        Severity = severity,
        Status = "Active",
        CreatedDate = new DateTime(year, month, day)
    };

    // Every demo patient gets the same five sample files (they live in App_Data/Documents).
    private static List<PatientDocument> StandardDocuments(int year, int month) => new()
    {
        Document("Discharge Summary", "Clinical Note", "discharge-summary.pdf", year, month, 22),
        Document("Comprehensive Metabolic Panel", "Lab Result", "lab-results.pdf", year, month, 18),
        Document("Chest X-Ray, PA and Lateral", "Imaging", "chest-xray.png", year, month, 17),
        Document("Cardiology Referral Letter", "Referral", "referral-letter.docx", year, month, 2),
        Document("Insurance Card (front)", "Administrative", "insurance-card.jpg", year, 1, 12)
    };

    private static PatientDocument Document(string name, string type, string fileName, int year, int month, int day) => new()
    {
        DocumentName = name,
        DocumentType = type,
        FilePath = fileName,
        DocumentDate = new DateTime(year, month, day),
        UploadedDate = new DateTime(year, month, day).AddHours(14)
    };
}
