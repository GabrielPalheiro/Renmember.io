namespace Renmember.Domain.Users;

public sealed class User
{
    public const int NameMaxLength = 100;
    public const int TimeZoneMaxLength = 64;

    private const int GuidVersion7 = 7;

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string TimeZone { get; private set; }

    private User(Guid id, string name, string timeZone)
    {
        Id = id;
        Name = name;
        TimeZone = timeZone;
    }

    public static User Criar(Guid id, string name, string timeZone)
    {
        GarantirIdVersao7(id);
        GarantirNomeValido(name);
        GarantirFusoIana(timeZone);

        return new User(id, name.Trim(), timeZone);
    }

    private static void GarantirIdVersao7(Guid id)
    {
        if (id.Version != GuidVersion7)
        {
            throw new ArgumentException("O Id do usuário deve ser um Guid versão 7.", nameof(id));
        }
    }

    private static void GarantirNomeValido(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > NameMaxLength)
        {
            throw new ArgumentException(
                $"O nome do usuário é obrigatório e tem no máximo {NameMaxLength} caracteres.",
                nameof(name));
        }
    }

    private static void GarantirFusoIana(string timeZone)
    {
        TimeZoneInfo.TryFindSystemTimeZoneById(timeZone, out var fuso);

        if (fuso is not { HasIanaId: true })
        {
            throw new ArgumentException(
                $"'{timeZone}' não é um fuso IANA válido, como 'America/Sao_Paulo'.",
                nameof(timeZone));
        }
    }
}
