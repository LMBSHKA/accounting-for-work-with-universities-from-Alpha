using Application.Students.Models;
using System.ComponentModel;

namespace API.Contracts.Students;

public class GetTeamsRequest
{
	[Description("""
	Количество команд для загрузки.
	Минимум: 1.
	Максимум: 100.
	""")]
	public int Limit { get; set; } = 8;

	[Description("""
	Смещение для пагинации.
	Например, если уже загружено 8 команд, передай offset = 8.
	""")]
	public int Offset { get; set; }

	[Description("""
	Поиск по названию команды, стеку, проекту или ФИО участника.
	Если null или пустая строка — поиск не применяется.
	""")]
	public string? Search { get; set; }

	[Description("Необязательный фильтр по проекту. Используется на форме назначения встречи после выбора проекта.")]
	public Guid? ProjectId { get; set; }

	[Description("""
	Фильтр команд.

	Значения:
	1 — All: все команды.
	2 — ActiveOnProject: команды активных проектов.
	3 — WithoutProject: команды без проекта, если такие данные есть в БД.
	""")]
	public TeamListFilter Filter { get; set; } = TeamListFilter.All;
}
