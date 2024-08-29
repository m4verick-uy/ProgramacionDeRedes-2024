# Programación de Redes - 2024

En C# y .NET, los bloques `try`, `catch`, y `finally` se utilizan para manejar excepciones y garantizar que se realice la limpieza de recursos, incluso si ocurre una excepción. Aquí te explico las diferencias:

### `try { } catch { }`:
- **Propósito**: El bloque `try` se utiliza para encapsular código que puede lanzar una excepción. El bloque `catch` se utiliza para manejar esa excepción.
- **Uso**: Cuando esperas que pueda ocurrir una excepción y quieres manejarla de alguna manera (por ejemplo, registrando el error, mostrando un mensaje al usuario, o tomando alguna acción correctiva), usas `try { } catch { }`.
- **Estructura**:
    ```csharp
    try
    {
        // Código que puede lanzar una excepción
    }
    catch (Exception ex)
    {
        // Código para manejar la excepción
    }
    ```

- **Ejemplo**:
    ```csharp
    try
    {
        int result = 10 / int.Parse("0");
    }
    catch (DivideByZeroException ex)
    {
        Console.WriteLine("No se puede dividir por cero.");
    }
    ```

### `try { } finally { }`:
- **Propósito**: El bloque `finally` se utiliza para ejecutar código que debe ejecutarse independientemente de si se lanza o no una excepción en el bloque `try`.
- **Uso**: Cuando tienes código que debe ejecutarse sin importar si ocurre una excepción o no (por ejemplo, liberar recursos como cerrar archivos, conexiones a bases de datos, etc.), usas `try { } finally { }`.
- **Estructura**:
    ```csharp
    try
    {
        // Código que puede lanzar una excepción
    }
    finally
    {
        // Código que se ejecuta siempre
    }
    ```

- **Ejemplo**:
    ```csharp
    StreamReader reader = null;
    try
    {
        reader = new StreamReader("archivo.txt");
        // Leer archivo
    }
    finally
    {
        if (reader != null)
            reader.Close(); // Asegura que el archivo se cierre
    }
    ```
orio agrupa el material de la asignatura "Programación de Redes" de 2024. Los recursos se organizan por práctico, incluyendo ejercicios de laboratorio, o por tema, con ejemplos y ejercicios relacionados.

## Autor
#### [**Guillermo Echichure**](mailto:gechichure@gmail.com)
