const state = {
    token: ""
};

const loginForm = document.getElementById("loginForm");
const loginStatus = document.getElementById("loginStatus");
const jwtToken = document.getElementById("jwtToken");
const currentUser = document.getElementById("currentUser");
const pingResult = document.getElementById("pingResult");
const prescriptionsBody = document.getElementById("prescriptionsBody");
const invoicesBody = document.getElementById("invoicesBody");
const actionButtonTemplate = document.getElementById("actionButtonTemplate");
const pingButton = document.getElementById("pingButton");
const refreshPrescriptionsButton = document.getElementById("refreshPrescriptionsButton");
const refreshInvoicesButton = document.getElementById("refreshInvoicesButton");
const copyTokenButton = document.getElementById("copyTokenButton");
const resetButton = document.getElementById("resetButton");

loginForm.addEventListener("submit", onLogin);
pingButton.addEventListener("click", loadPing);
refreshPrescriptionsButton.addEventListener("click", loadPrescriptions);
refreshInvoicesButton.addEventListener("click", loadInvoices);
copyTokenButton.addEventListener("click", copyToken);
resetButton.addEventListener("click", resetView);

async function onLogin(event) {
    event.preventDefault();

    const username = document.getElementById("username").value.trim();
    const password = document.getElementById("password").value;

    if (!username || !password) {
        loginStatus.textContent = "Fyll ut brukernavn og passord.";
        return;
    }

    setStatus("Logger inn...");

    const query = new URLSearchParams({ username, password });
    const response = await fetch(`/Login/Login?${query.toString()}`, {
        method: "POST",
        credentials: "include"
    });

    if (!response.ok) {
        const message = await safeReadText(response);
        setStatus(`Innlogging feilet (${response.status}): ${message || "Ukjent feil"}`);
        return;
    }

    state.token = await response.text();
    jwtToken.value = state.token;
    setStatus("Innlogging vellykket. Cookie er satt. Laster brukerdata og dokumenter...");

    await Promise.all([loadCurrentUser(), loadPrescriptions(), loadInvoices()]);
}

async function loadPing() {
    pingResult.textContent = "Laster...";

    const response = await fetch("/System/Ping", {
        method: "GET"
    });

    if (!response.ok) {
        pingResult.textContent = `Feil ${response.status}`;
        return;
    }

    pingResult.textContent = await response.text();
}

async function loadCurrentUser() {
    const response = await fetch("/Login/CurrentUser", {
        method: "GET",
        credentials: "include"
    });

    if (!response.ok) {
        currentUser.textContent = `Kunne ikke hente bruker (${response.status}).`;
        return;
    }

    const user = await response.json();
    currentUser.textContent = JSON.stringify(user, null, 2);
}

async function loadPrescriptions() {
    prescriptionsBody.innerHTML = "<tr><td colspan=\"5\">Laster...</td></tr>";

    const response = await fetch("/Prescription/GetMyPrescriptions", {
        method: "GET",
        credentials: "include"
    });

    if (!response.ok) {
        prescriptionsBody.innerHTML = `<tr><td colspan=\"5\">Kunne ikke hente resepter (${response.status})</td></tr>`;
        return;
    }

    const rows = await response.json();
    if (!rows || rows.length === 0) {
        prescriptionsBody.innerHTML = "<tr><td colspan=\"5\">Ingen resepter funnet</td></tr>";
        return;
    }

    prescriptionsBody.innerHTML = "";
    for (const row of rows) {
        const tr = document.createElement("tr");

        tr.appendChild(cell(row.id));
        tr.appendChild(cell(row.docNo));
        tr.appendChild(cell(formatDate(row.prescriptionDate)));
        tr.appendChild(cell(row.medication));

        const actionCell = document.createElement("td");
        const button = createActionButton("Last ned PDF", async () => {
            await downloadFile(`/Prescription/GetPDF?id=${encodeURIComponent(row.id)}`, `Resept-${row.id}.pdf`);
        });
        actionCell.appendChild(button);
        tr.appendChild(actionCell);

        prescriptionsBody.appendChild(tr);
    }
}

async function loadInvoices() {
    invoicesBody.innerHTML = "<tr><td colspan=\"5\">Laster...</td></tr>";

    const response = await fetch("/Invoice/GetMyInvoices", {
        method: "GET",
        credentials: "include"
    });

    if (!response.ok) {
        invoicesBody.innerHTML = `<tr><td colspan=\"5\">Kunne ikke hente fakturaer (${response.status})</td></tr>`;
        return;
    }

    const rows = await response.json();
    if (!rows || rows.length === 0) {
        invoicesBody.innerHTML = "<tr><td colspan=\"5\">Ingen fakturaer funnet</td></tr>";
        return;
    }

    invoicesBody.innerHTML = "";
    for (const row of rows) {
        const tr = document.createElement("tr");

        tr.appendChild(cell(row.id));
        tr.appendChild(cell(row.invoiceNo));
        tr.appendChild(cell(formatDate(row.invoiceDate)));
        tr.appendChild(cell(formatDate(row.dueDate)));

        const actionCell = document.createElement("td");
        const button = createActionButton("Last ned PDF", async () => {
            await downloadFile(`/Invoice/GetInvoicePDF?filename=${encodeURIComponent(row.filename)}`, row.filename || `Faktura-${row.id}.pdf`);
        });
        actionCell.appendChild(button);
        tr.appendChild(actionCell);

        invoicesBody.appendChild(tr);
    }
}

async function downloadFile(url, fallbackName) {
    const response = await fetch(url, {
        method: "GET",
        credentials: "include"
    });

    if (!response.ok) {
        setStatus(`Nedlasting feilet (${response.status}).`);
        return;
    }

    const blob = await response.blob();
    const contentDisposition = response.headers.get("Content-Disposition") || "";
    const filename = parseFilenameFromHeader(contentDisposition) || fallbackName;

    const blobUrl = URL.createObjectURL(blob);
    const anchor = document.createElement("a");
    anchor.href = blobUrl;
    anchor.download = filename;
    document.body.appendChild(anchor);
    anchor.click();
    anchor.remove();
    URL.revokeObjectURL(blobUrl);
}

function createActionButton(label, onClick) {
    const fragment = actionButtonTemplate.content.cloneNode(true);
    const button = fragment.querySelector("button");
    button.textContent = label;
    button.addEventListener("click", onClick);
    return button;
}

function cell(value) {
    const td = document.createElement("td");
    td.textContent = String(value ?? "");
    return td;
}

function formatDate(value) {
    const date = new Date(value);
    if (Number.isNaN(date.valueOf())) {
        return "-";
    }

    return new Intl.DateTimeFormat("no-NO", {
        dateStyle: "medium"
    }).format(date);
}

function parseFilenameFromHeader(contentDisposition) {
    const match = /filename=\"?([^\";]+)\"?/i.exec(contentDisposition);
    return match ? match[1] : "";
}

async function copyToken() {
    if (!state.token) {
        setStatus("Ingen token å kopiere. Logg inn først.");
        return;
    }

    try {
        await navigator.clipboard.writeText(state.token);
        setStatus("JWT kopiert til utklippstavlen.");
    } catch {
        setStatus("Kunne ikke kopiere token. Nettleseren blokkerte handlingen.");
    }
}

function resetView() {
    state.token = "";
    loginForm.reset();
    jwtToken.value = "";
    currentUser.textContent = "Ikke hentet";
    pingResult.textContent = "Ikke kjort ennå";
    prescriptionsBody.innerHTML = "<tr><td colspan=\"5\">Ingen data lastet</td></tr>";
    invoicesBody.innerHTML = "<tr><td colspan=\"5\">Ingen data lastet</td></tr>";
    setStatus("Visningen er nullstilt. Eventuell auth-cookie i nettleseren beholdes av serveren.");
}

function setStatus(message) {
    loginStatus.textContent = message;
}

async function safeReadText(response) {
    try {
        return await response.text();
    } catch {
        return "";
    }
}

loadPing();
