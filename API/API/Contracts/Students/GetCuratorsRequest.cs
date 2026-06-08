using System.ComponentModel;

namespace API.Contracts.Students;

public class GetCuratorsRequest
{
	[Description("Количество кураторов для загрузки. Минимум: 1. Максимум: 100.")]
	public int Limit { get; set; } = 8;

	[Description("Смещение для infinite scroll. Например, если уже загружено 8 кураторов, передай offset = 8.")]
	public int Offset { get; set; }

	[Description("Поиск по ФИО или почте куратора. Если null или пустая строка — поиск не применяется.")]
	public string? Search { get; set; }
}
