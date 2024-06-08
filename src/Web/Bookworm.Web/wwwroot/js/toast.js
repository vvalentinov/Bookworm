const toastQueue = [];
let activeToasts = 0;
const maxToasts = 4;

export function createToast(toastTitleMsg, toastBodyMsg, toastBgColor) {
    if (activeToasts >= maxToasts) {
        toastQueue.push({ toastTitleMsg, toastBodyMsg, toastBgColor });
        return;
    }

    showToast(toastTitleMsg, toastBodyMsg, toastBgColor);
}

function showToast(toastTitleMsg, toastBodyMsg, toastBgColor) {
    activeToasts++;

    const toastContainer = document.querySelector('.toast-container');

    const toastEl = document.createElement('div');
    toastEl.className = 'toast';
    toastEl.role = 'alert';
    toastEl.setAttribute('aria-live', 'assertive');
    toastEl.setAttribute('aria-atomic', 'true');

    const toastHeaderEl = document.createElement('div');
    toastHeaderEl.className = `toast-header ${toastBgColor}`;
    const toastTitle = document.createElement('strong');
    toastTitle.className = 'me-auto';
    toastTitle.textContent = toastTitleMsg;
    const toastBtn = document.createElement('button');
    toastBtn.type = 'button';
    toastBtn.className = 'btn-close';
    toastBtn.setAttribute('data-bs-dismiss', 'toast');
    toastBtn.setAttribute('aria-label', 'Close');
    toastHeaderEl.appendChild(toastTitle);
    toastHeaderEl.appendChild(toastBtn);

    const toastBodyEl = document.createElement('div');
    toastBodyEl.className = 'toast-body';
    toastBodyEl.textContent = toastBodyMsg;

    toastEl.appendChild(toastHeaderEl);
    toastEl.appendChild(toastBodyEl);

    toastContainer.appendChild(toastEl);

    const toastBootstrap = bootstrap.Toast.getOrCreateInstance(toastEl);
    toastBootstrap.show();

    toastEl.addEventListener('hidden.bs.toast', () => {
        toastContainer.removeChild(toastEl);
        activeToasts--;
        if (toastQueue.length > 0) {
            const nextToast = toastQueue.shift();
            showToast(nextToast.toastTitleMsg, nextToast.toastBodyMsg, nextToast.toastBgColor);
        }
    });
}


