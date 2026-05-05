using Entities.enums;
using System.ComponentModel;

namespace API.Contracts.Discussions;

public class GetDiscussionIdeasRequest
{
    [Description("""
    Количество идей для загрузки.
    Минимум: 1.
    Максимум: 100.
    """)]
    public int Limit { get; set; } = 8;

    [Description("""
    Смещение для пагинации.
    Например, если уже загружено 10 идей, передай offset = 10.
    """)]
    public int Offset { get; set; }

    [Description("""
    Поиск по названию идеи.
    Если null или пустая строка — поиск не применяется.
    """)]
    public string? Search { get; set; }

    [Description("""
    Фильтр по статусам идеи.

    Значения:
    1 — Active: активная идея.
    2 — Rejected: отклоненная идея.
    3 — Archived: архивная идея.
    4 — Completed: завершенная идея.

    Можно передать несколько значений, например: [1, 3].
    Если null или пустой массив — вернутся идеи со всеми статусами.
    """)]
    public List<ProjectStatus>? Statuses { get; set; }
}
