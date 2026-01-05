# Mi Agenda - Sistema de Gestión de Contactos

Sistema web de gestión de contactos desarrollado con ASP.NET Core MVC siguiendo principios de arquitectura en capas y separación clara de responsabilidades. Implementa autenticación segura sin Identity, gestión completa de contactos (CRUD) y recuperación de contraseñas mediante correo electrónico.

---

## Características Principales

### Seguridad y Autenticación
- **Autenticación personalizada** sin ASP.NET Identity
- **Cookie-based authentication** con Claims Principal
- **Hashing de contraseñas** con Argon2id 
- **Recuperación de contraseña** mediante tokens seguros por correo electrónico
- **Tokens hasheados** en base de datos (nunca almacenados en texto plano)
- **Validación de propiedad** de recursos (usuarios solo acceden a sus propios contactos)
- **Protección CSRF** con Anti-Forgery Tokens
- **Mensajes genéricos** para prevenir enumeración de usuarios

### Sistema de Correo Electrónico
- **Envío de correos** mediante SMTP
- **Recuperación de contraseña** con enlaces seguros
- **Configuración segura** mediante User Secrets

### Gestión de Contactos
- **CRUD completo** (Crear, Leer, Actualizar, Eliminar)
- **Información del contacto**: nombre, apellidos, teléfono, fecha de nacimiento
- **Cálculo automático** de edad
- **Redes sociales** asociadas a cada contacto
- **Ordenamiento** alfabético automático
- **Validación de datos** tanto en cliente como en servidor

---

## Responsabilidades por Capa

### ServicesUI (Presentation Layer)
**Responsabilidad**: Interacción con el usuario

- ✅ Recibir y validar entrada del usuario
- ✅ Mapear entre ViewModels y Entities
- ✅ Gestionar cookies de autenticación
- ✅ Trabajar con Claims Principal
- ✅ Manejar try-catch y mostrar mensajes al usuario
- ✅ Renderizar vistas con Razor
- ✅ Aplicar validaciones del lado del cliente

**NO**:
- ❌ Lógica de negocio
- ❌ Accede directamente a la base de datos
- ❌ Conoce detalles de implementación de capas inferiores

---

### Infrastructure (Business Logic Layer)
**Responsabilidad**: Lógica de negocio 

- ✅ Implementar reglas de negocio
- ✅ Validar que las operaciones sean permitidas
- ✅ Múltiples operaciones del repositorio
- ✅ Proveer servicios de aplicación (Email, Hashing)
- ✅ Calcular valores derivados (edad a partir de fecha de nacimiento)

**NO**:
- ❌ No conoce ViewModels (pertenecen a la capa UI)
- ❌ No manejar HTTP requests/responses
- ❌ No contiene lógica de presentación

---

### DataAccess (Data Layer)
**Responsabilidad**: Acceso a datos

- ✅ Implementar operaciones CRUD
- ✅ Ejecutar queries a la base de datos
- ✅ Mapear entre objetos y tablas (ORM)
- ✅ Gestionar transacciones si es necesario

**NO**:
- ❌ No lógica de negocio
- ❌ No conoce ViewModels
- ❌ No maneja try-catch (las excepciones suben a capas superiores)

---

## Tecnologías y Patrones Implementados

## Stack Tecnológico

| Categoría | Tecnología | Versión | Propósito |
|-----------|------------|---------|-----------|
|**Backend** | ASP.NET Core | 9.0 | Framework web |
|**ORM** | Entity Framework Core | 9.0 | Acceso a datos |
|**Base de Datos** | SQL Server | 2019+ | Persistencia |
|**Seguridad** | Argon2id | - | Hashing de contraseñas |
|**Frontend** | Razor Views (MVC) | - | Motor de vistas |
|**Frontend** | Jquery | Validaciones del lado del cliente |
|**CSS** | Bootstrap | 5.3 | Diseño responsive |
|**Validación** | jQuery Validation | 1.19 | Validación cliente |
|**Email** | System.Net.Mail | - | Envío SMTP |

### Patrones de Diseño
- **Repository Pattern** - Abstracción del acceso a datos
- **Dependency Injection** - Inyección de dependencias nativa de .NET
- **ViewModel Pattern** - Separación entre modelos de dominio y presentación
- **Base Controller Pattern** - Reutilización de funcionalidad común

### Principios SOLID
- **Single Responsibility** - Cada clase tiene una única responsabilidad
- **Dependency Inversion** - Dependencia de abstracciones (interfaces)
- **Separation of Concerns** - Separación clara entre capas

---

## Implementación de Seguridad

### Tokens de Recuperación
- Generación con `RandomNumberGenerator` (criptográficamente seguro)
- Hash SHA256 antes de almacenar en base de datos
- Expiración de 1 hora
- Invalidación de tokens previos al generar uno nuevo
- Marcado como "usado" después de resetear contraseña
- Autenticación basada en Cookies con Claims (sin ASP.NET Identity)

---

## Flujo de Usuario

### Registro y Autenticación
1. Usuario se registra con email y contraseña
2. Contraseña se hashea con Argon2id antes de guardarse
3. Usuario inicia sesión con credenciales
4. Se crea cookie de autenticación con Claims
5. Usuario accede a su agenda de contactos

### Recuperación de Contraseña
1. Usuario solicita recuperación desde "Forgot Password"
2. Sistema genera token seguro y lo hashea
3. Se envía email con enlace temporal
4. Usuario hace clic en el enlace (válido 1 hora)
5. Ingresa nueva contraseña
6. Token se marca como usado y contraseña se actualiza

### Gestión de Contactos
1. Usuario autenticado ve solo sus contactos
2. Puede crear, editar y eliminar contactos
3. Sistema valida que el contacto pertenezca al usuario
4. Edad se calcula automáticamente

---

## Funcionalidades por Rol

### Usuario Estándar
- Gestión completa de sus propios contactos
- Recuperación de contraseña
- Edición de perfil

### Administrador (Próxima implementación)
- Gestión de usuarios
- Visualización de logs
- Configuración del sistema

---

## Seguridad - Checklist

- Contraseñas hasheadas con Argon2id
- Tokens de recuperación hasheados con SHA256
- Cookies HttpOnly y Secure
- Protección CSRF con Anti-Forgery Tokens
- Validación de propiedad de recursos
- Mensajes genéricos (anti-enumeración)
- User Secrets para datos sensibles
- SQL injection prevenido (EF Core parametrizado)
- XSS prevenido (Razor encode automático)

## Implementación pendiente
- Rate limiting (pendiente)
- Two-Factor Authentication (pendiente)
- Captcha en login (pendiente)

---

## Mejoras Futuras

### Corto Plazo
- Sistema de roles y permisos
- Confirmación de email al registrarse
- Gestión de redes sociales (agregar/eliminar)
- Subida de fotos de contactos
- Exportar contactos a CSV/Excel

### Mediano Plazo
- Autenticación de dos factores (2FA)
- Rate limiting para prevenir ataques
- Historial de cambios en contactos
- Búsqueda y filtrado avanzado
- API REST para consumo externo

### Largo Plazo
- Aplicación móvil (Xamarin/MAUI)
- Sincronización con Google Contacts
- Grupos de contactos
- Recordatorios de cumpleaños
- Dashboard con estadísticas

---

## Licencia

Este proyecto está bajo la Licencia MIT. Ver `LICENSE` para más información.

---

## Autor

**[Carlos García Software Developer (.NET)]**
- LinkedIn: [carlosdevelopp](https://linkedin.com/in/carlosdevel)
- GitHub: [Carlosdevelopp](https://github.com/Carlosdevelopp)
- Email: carlosdevelopp@gmail.com

---

## Agradecimientos

- Documentación oficial de [ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- Guías de seguridad de [OWASP](https://owasp.org/)
- Comunidad de desarrolladores .NET

---

## Screenshots

### Página de Login
![Login](docs/screenshots/login.png)

### Registo de Usuario
![Editar](docs/screenshots/registro.png)

### Recuperación de Contraseña
![Editar](docs/screenshots/recuperarContraseña.png)

### Agenda de Contactos
![Agenda](docs/screenshots/agenda.png)

