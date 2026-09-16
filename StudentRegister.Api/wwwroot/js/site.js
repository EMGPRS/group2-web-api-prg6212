const apiUrl = "/api/Student";
const tableBody = document.getElementById("studentsTableBody");
const emptyState = document.getElementById("emptyState");
const loadingState = document.getElementById("loadingState");
const recordCount = document.getElementById("recordCount");
const searchInput = document.getElementById("searchInput");
const clearSearchButton = document.getElementById("clearSearchButton");
const feedback = document.getElementById("feedback");
const studentModal = new bootstrap.Modal("#studentModal");
const deleteModal = new bootstrap.Modal("#deleteModal");
let studentToDelete;
let searchTimer;

function renderIcons() { lucide.createIcons(); }
function displayFeedback(message, type = "success") { feedback.className = `alert alert-${type} mt-4 mb-0`; feedback.textContent = message; }
function getGenderLabel(gender) { return { F: "Female", M: "Male" }[gender] || gender; }
function renderStudents(students) {
    loadingState.classList.add("d-none");
    tableBody.innerHTML = students.map(student => `<tr><td><span class="student-name">${escapeHtml(student.firstName)} ${escapeHtml(student.lastName)}</span><span class="student-id">ID ${student.id}</span></td><td>${escapeHtml(student.studentNumber)}</td><td><span class="gender-badge">${escapeHtml(getGenderLabel(student.gender))}</span></td><td><div class="table-actions"><button class="icon-button" type="button" data-action="edit" data-id="${student.id}" aria-label="Edit ${escapeHtml(student.firstName)} ${escapeHtml(student.lastName)}" title="Edit student"><i data-lucide="pencil"></i></button><button class="icon-button delete" type="button" data-action="delete" data-id="${student.id}" aria-label="Delete ${escapeHtml(student.firstName)} ${escapeHtml(student.lastName)}" title="Delete student"><i data-lucide="trash-2"></i></button></div></td></tr>`).join("");
    emptyState.classList.toggle("d-none", students.length !== 0);
    recordCount.textContent = `${students.length} ${students.length === 1 ? "student" : "students"}`;
    renderIcons();
}
function escapeHtml(value) { const element = document.createElement("div"); element.textContent = value ?? ""; return element.innerHTML; }
async function loadStudents() {
    loadingState.classList.remove("d-none"); emptyState.classList.add("d-none");
    const search = searchInput.value.trim(); clearSearchButton.classList.toggle("d-none", !search);
    try {
        const response = await fetch(search ? `${apiUrl}/search?search=${encodeURIComponent(search)}` : apiUrl);
        if (response.status === 404) { renderStudents([]); return; }
        if (!response.ok) throw new Error();
        renderStudents(await response.json());
    } catch { loadingState.classList.add("d-none"); displayFeedback("The register could not be loaded. Please try again.", "danger"); }
}
function openStudentModal(student) {
    document.getElementById("studentForm").reset();
    document.getElementById("studentId").value = student?.id ?? "";
    document.getElementById("modalEyebrow").textContent = student ? "Update record" : "New record";
    document.getElementById("studentModalTitle").textContent = student ? "Edit student" : "Add student";
    if (student) ["firstName", "lastName", "studentNumber", "gender"].forEach(field => document.getElementById(field).value = student[field]);
    studentModal.show();
}
document.getElementById("addStudentButton").addEventListener("click", () => openStudentModal());
searchInput.addEventListener("input", () => { clearTimeout(searchTimer); searchTimer = setTimeout(loadStudents, 250); });
clearSearchButton.addEventListener("click", () => { searchInput.value = ""; loadStudents(); searchInput.focus(); });
tableBody.addEventListener("click", async event => {
    const button = event.target.closest("button[data-action]"); if (!button) return;
    const response = await fetch(`${apiUrl}/${button.dataset.id}`); if (!response.ok) return displayFeedback("That student could not be found.", "warning");
    const student = await response.json();
    if (button.dataset.action === "edit") openStudentModal(student);
    else { studentToDelete = student; document.getElementById("deleteStudentName").textContent = `${student.firstName} ${student.lastName}`; deleteModal.show(); }
});
document.getElementById("studentForm").addEventListener("submit", async event => {
    event.preventDefault(); const id = document.getElementById("studentId").value;
    const student = Object.fromEntries(["firstName", "lastName", "studentNumber", "gender"].map(field => [field, document.getElementById(field).value.trim()]));
    const response = await fetch(id ? `${apiUrl}/${id}` : apiUrl, { method: id ? "PUT" : "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify(student) });
    if (!response.ok) return displayFeedback("The student could not be saved. Check the details and try again.", "danger");
    studentModal.hide(); displayFeedback(id ? "Student details updated." : "Student added to the register."); loadStudents();
});
document.getElementById("confirmDeleteButton").addEventListener("click", async () => {
    const response = await fetch(`${apiUrl}/${studentToDelete.id}`, { method: "DELETE" });
    if (!response.ok) return displayFeedback("The student could not be deleted.", "danger");
    deleteModal.hide(); displayFeedback(`${studentToDelete.firstName} ${studentToDelete.lastName} was removed from the register.`); loadStudents();
});
renderIcons(); loadStudents();