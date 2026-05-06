# Changelog

Todos los cambios notables en este proyecto serán documentados en este archivo.
## [1.2.2.4] - 2026-05-04
### Modificado
- Se modifico la clase para ignorar los registros marcados como eliminados en una tabla.

## [1.2.2.4] - 2026-05-05
### Ajuste
- Se ajusto la lectura de campos Memo para la correcta lectura de campos string o campos by[]

## [1.2.2.3] - 2026-05-01
### Agregado
- Se agrego README.md con información de la librería y su uso.
- Se agrego CHANGELOG.md con el historial de cambios de la librería.

## [1.2.2.2] - 2026-05-01

### Agregado
- Posibilidad de leer campos Blob como tipo Byte[] donde usualmente se guardan fotos, imágenes, etc. Tambien corrección en lectura de campos Memo como string

## [1.2.2.1] - 2026-04-30

### Ajuste
- Ajuste en los campos string para no eliminar los espacios en blanco al inicio de la cadena, se cambio Trim() por TrimEnd().

---

## [1.2.2] - 2025-01-01
### Agregado
- Se agrego DbfFieldAttribute para poder mapear los registros leidos
- Tambien se agregaron la extensión de AddEntities() y GetEntities().

---
## [1.2.1] - Original