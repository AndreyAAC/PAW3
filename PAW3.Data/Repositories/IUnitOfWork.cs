// PAW3.Data/Repositories/IUnitOfWork.cs
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace PAW3.Data.Repositories
{
    /// IMPLEMENTACION DE UNIT OF WORK PATTERN
    /// Coordina la confirmación de transacciones (SaveChanges) del DbContext.


    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}

/* 
    Explicacion rapida (para tu informe/documentacion):

   Donde se implementa el patron Repository:

      - En la clase "PAW3.Data.Repositories.RepositoryProduct", que encapsula
        el acceso a datos de la entidad "Product" sin exponer detalles
        de EF Core a las capas superiores.

      - En la clase "PAW3.Data.Repositories.UnitOfWork", que centraliza
        la confirmacion de transacciones mediante "SaveChangesAsync".
        Los repositorios evitan la llamada a SaveChanges directamente; el commit ocurre
        aqui para agrupar cambios en una sola transaccion.

   ¿Por que se eligio el patron Repository?

      Elegi este patron para coordinar multiples operaciones de 
      datos dentro de una misma transaccion y mantener consistencia.
      Tambien para preveenir que la base de datos tenga estados de 
      inconsistencia y asi todos los cambio apliquen de una o ninguno aplique.
      Asimismo, este patron ayuda a que el trafico en la red sea reducido al hacer un
      solo llamado en vez de varios llamados que crean mas trafico de datos.
*/