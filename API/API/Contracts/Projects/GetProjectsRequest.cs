using Entities.enums;
using System.ComponentModel;

public class GetProjectsRequest
{
	[Description("""
    Количество проектов для загрузки.
    Минимум: 1.
    Максимум: 100.
    """)]
	public int Limit { get; set; }

	[Description("""
    Смещение для пагинации.
    Например, если уже загружено 10 проектов, передай offset = 10.
    """)]
	public int Offset { get; set; }

	[Description("""
    Поиск по названию проекта.
    Если null или пустая строка — поиск не применяется.
    """)]
	public string? Search { get; set; }

	[Description("""
    Фильтр по статусам проекта.

    Значения:
    1 — Active: активный проект.
    2 — Rejected: отклоненный проект.
    3 — Archived: архивный проект.
    4 — Completed: завершенный проект.

    Можно передать несколько значений, например: [1, 3].
    Если null или пустой массив — вернутся проекты со всеми статусами.
    """)]
	public List<ProjectStatus>? Statuses { get; set; }
}