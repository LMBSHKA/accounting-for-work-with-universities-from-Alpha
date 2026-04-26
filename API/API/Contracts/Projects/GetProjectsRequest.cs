using Entities.enums;
using System.ComponentModel.DataAnnotations;

namespace API.Contracts.Projects;

public class GetProjectsRequest
{
    /// <summary>
    /// Поиск по названию проекта.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// Список статусов для фильтрации. Если пусто или null — возвращаются все проекты.
    /// Например: Active, Archived, Completed, Rejected.
    /// </summary>
    public List<ProjectStatus>? Statuses { get; set; }

    /// <summary>
    /// Сколько проектов пропустить. Для кнопки "Загрузить еще" передавайте количество уже загруженных карточек.
    /// </summary>
    [Range(0, int.MaxValue)]
    public int Offset { get; set; } = 0;

    /// <summary>
    /// Сколько проектов вернуть за один запрос.
    /// </summary>
    [Range(1, 100)]
    public int Limit { get; set; } = 8;
}
