document.addEventListener("DOMContentLoaded", () => {

    /* Preview foto de perfil */
    const fotoInput = document.getElementById("fotoUsuarioInput");
    const fotoPreview = document.getElementById("fotoPreview");
    const fotoError = document.getElementById("fotoError");

    if (fotoInput && fotoPreview) {
        fotoInput.addEventListener("change", () => {
            const file = fotoInput.files[0];
            fotoError.textContent = "";

            if (!file) return;

            const validTypes = ["image/jpeg", "image/png", "image/gif"];
            const maxSize = 5 * 1024 * 1024; // 5MB

            if (!validTypes.includes(file.type)) {
                fotoError.textContent = "Formato no válido. Usa JPG, PNG o GIF.";
                fotoInput.value = "";
                return;
            }

            if (file.size > maxSize) {
                fotoError.textContent = "La imagen no debe superar los 5MB.";
                fotoInput.value = "";
                return;
            }

            const reader = new FileReader();
            reader.onload = e => fotoPreview.src = e.target.result;
            reader.readAsDataURL(file);
        });
    }


    /* Mostrar / Ocultar Password */
    const passwordInput = document.getElementById("password");
    const togglePasswordBtn = document.getElementById("togglePassword");
    const togglePasswordIcon = document.getElementById("togglePasswordIcon");

    if (passwordInput && togglePasswordBtn) {
        togglePasswordBtn.addEventListener("click", () => {
            const isHidden = passwordInput.type === "password";
            passwordInput.type = isHidden ? "text" : "password";
            togglePasswordIcon.className = isHidden ? "bi bi-eye-slash" : "bi bi-eye";
        });
    }

    /* Fuerza de contraseña */
    const strengthBar = document.getElementById("passwordStrengthBar");
    const strengthText = document.getElementById("passwordStrengthText");

    if (passwordInput && strengthBar) {
        passwordInput.addEventListener("input", () => {
            const value = passwordInput.value;
            strengthBar.className = "password-strength-bar";
            strengthText.textContent = "";

            if (value.length === 0) return;

            if (value.length < 6) {
                strengthBar.classList.add("strength-weak");
                strengthText.textContent = "Contraseña débil";
            } else if (value.length < 10) {
                strengthBar.classList.add("strength-medium");
                strengthText.textContent = "Contraseña media";
            } else {
                strengthBar.classList.add("strength-strong");
                strengthText.textContent = "Contraseña fuerte";
            }
        });
    }

    /* Confirmación password */
    const confirmPassword = document.getElementById("confirmPassword");
    const matchError = document.getElementById("passwordMatchError");

    if (passwordInput && confirmPassword) {
        confirmPassword.addEventListener("input", () => {
            matchError.textContent =
                confirmPassword.value !== passwordInput.value
                    ? "Las contraseñas no coinciden"
                    : "";
        });
    }

    /* Loanding overlay */
    const form = document.getElementById("registerForm");
    const overlay = document.getElementById("loadingOverlay");

    if (form && overlay) {
        form.addEventListener("submit", () => {
            overlay.classList.add("show");
        });
    }
});
