using Application.Students.Models;
using System.ComponentModel;

namespace API.Contracts.Students;

public class GetStudentsRequest
{
	[Description("""
	Количество студентов для загрузки.
	Минимум: 1.
	Максимум: 100.
	""")]
	public int Limit { get; set; } = 8;

	[Description("""
	Смещение для пагинации.
	Например, если уже загружено 8 студентов, передай offset = 8.
	""")]
	public int Offset { get; set; }

	[Description("""
	Поиск по ФИО студента, email или названию команды.
	Если null или пустая строка — поиск не применяется.
	""")]
	public string? Search { get; set; }

	[Description("""
	Фильтр студентов.

	Значения:
	1 — All: все студенты.
	2 — ActiveOnProject: студенты, которые находятся хотя бы в одной команде активного проекта.
	3 — WithoutProject: студенты без команды/проекта.
	""")]
	public StudentListFilter Filter { get; set; } = StudentListFilter.All;
}
