using Dressly.Application.Ports.Input;

namespace Dressly.Application.Services;

public class GeneradorAvatarService
{
    private readonly IAlmacenamientoImagenes _almacenamiento;
    // Aquí podrías inyectar tu cliente HTTP para la API de IA (Gemini API, etc.)

    public GeneradorAvatarService(IAlmacenamientoImagenes almacenamiento)
    {
        _almacenamiento = almacenamiento;
    }

    public async Task<string> GenerarYGuardarAvatarAsync(byte[] fotoOriginalBytes, string nombreArchivo)
    {
        // 1. Instanciar el request con el prompt estructurado
        var request = new ModelSheetRequest
        {
            SourceImagePath = nombreArchivo
        };

        // 2. [Simulación o llamada real a la IA]: 
        // Envías `fotoOriginalBytes` junto con `request.PromptText` a la API de Gemini.
        // La API te devolverá una nueva imagen generada en bytes (la hoja de modelo 2D).
        byte[] bytesAvatarGenerado = await LlamarApiDeInteligenciaArtificialAsync(fotoOriginalBytes, request.PromptText);

        // 3. Usar LA INTERFAZ QUE YA TIENES (`IAlmacenamientoImágenes`) para guardarla físicamente
        string urlAvatarFinal = await _almacenamiento.GuardarAsync(bytesAvatarGenerado, "model_sheet_" + nombreArchivo);

        return urlAvatarFinal; // Esta URL es la que guardarás en PerfilFisico.FotoUrl
    }

    private async Task<byte[]> LlamarApiDeInteligenciaArtificialAsync(byte[] imagenBytes, string prompt)
    {
        // TODO: Implementar la llamada real al endpoint de la API de IA con soporte de imágenes
        await Task.CompletedTask;
        return imagenBytes; // Retorno temporal de prueba
    }
}