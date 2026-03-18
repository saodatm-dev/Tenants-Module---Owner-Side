using Building.Domain.Units.Events;
using Core.Application.Abstractions.Services.Minio;
using Core.Domain.Events;

namespace Building.Application.Units.Events;

internal sealed class RemoveUnitDomainEventHandler(
	IFileManager fileManager) : IDomainEventHandler<RemoveUnitDomainEvent>
{
	public async ValueTask Handle(RemoveUnitDomainEvent @event, CancellationToken cancellationToken)
	{
		#region Images

		if (@event.Unit.Images?.Count() > 0)
		{
			// copying to tenant bucket
			var objectNameTasks = @event.Unit.Images.Select(image => fileManager.DeleteFileAsync(image, cancellationToken));

			await Task.WhenAll(objectNameTasks);
		}

		#endregion

		await ValueTask.CompletedTask;
	}
}
