using Dressly.Application.Ports.Input; // Aquí vive tu interfaz IAlmacenamientoImagenes

namespace Dressly.Application.Services;

public class PerfilService
{
    private readonly IAlmacenamientoImagenes _almacenamientoImagenes;
    private readonly GeneradorAvatarService _generadorAvatarService;
    // private readonly IUnitOfWork _unitOfWork; // O tu DbContext según uses

    public PerfilService(
        IAlmacenamientoImagenes almacenamientoImagenes,
        GeneradorAvatarService generadorAvatarService)
    {
        _almacenamientoImagenes = almacenamientoImagenes;
        _generadorAvatarService = generadorAvatarService;
    }

    public async Task ActualizarPerfilConAvatarAsync(int usuarioId, byte[] bytesArchivo, string nombreArchivo)
    {
        // 1. Generar el avatar con IA
        string urlGenerada = await _generadorAvatarService.GenerarYGuardarAvatarAsync(bytesArchivo, nombreArchivo);

        // 2. Buscar el perfil físico en tu base de datos
        // var perfilFisico = await _unitOfWork.PerfilesFisicos.ObtenerPorUsuarioIdAsync(usuarioId);

        // if (perfilFisico != null)
        // {
        //     // 3. Asignar la URL que devolvió el servicio de almacenamiento/IA
        //     perfilFisico.FotoUrl = urlGenerada; 
        //     
        //     // 4. Guardar cambios
        //     _unitOfWork.PerfilesFisicos.Actualizar(perfilFisico);
        //     await _unitOfWork.CompleteAsync();
        // }
    }
}