
# dBASE.NET

Notas de la versión

- 1.2.1. Original
- 1.2.2. Se agrego DbfFieldAttribute para poder mapear los registros leidos. Tambien se agregaron la extensión de AddEntities() y GetEntities().
- 1.2.2.1. Ajuste en los campos string para no eliminar los espacios en blanco al inicio de la cadena, se cambio Trim() por TrimEnd().
- 1.2.2.2. Posibilidad de leer campos Blob como tipo Byte[] donde usualmente se guardan fotos, imágenes, etc. Tambien corrección en lectura de campos Memo como string