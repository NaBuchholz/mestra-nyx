using MestraNyx.Domain.Exceptions;

namespace MestraNyx.Domain.Entities;

/// <summary>
/// Entidade que representa uma campanha de RPG
/// </summary>
public class Campaign
{
    // Propriedades com setters privados = encapsulamento
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string System { get; private set; } = "Call of Cthulhu 7e";
    public string? TimePeriod { get; private set; }
    public string? CoverImage { get; private set; }
    public bool IsPublic { get; private set; }
    public bool IsActive { get; private set; }
    public Guid OwnerId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Construtor privado = força uso do Factory Method
    private Campaign()
    {
    }

    /// <summary>
    /// Factory Method - única forma de criar uma campanha
    /// Garante que toda campanha criada é válida
    /// </summary>
    public static Campaign Create(
        string name,
        Guid ownerId,
        string? description = null,
        string? system = null,
        string? timePeriod = null)
    {
        // Validações de domínio
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome da campanha não pode ser vazio");

        if (name.Length > 200)
            throw new DomainException("Nome da campanha não pode ter mais de 200 caracteres");

        if (ownerId == Guid.Empty)
            throw new DomainException("OwnerId não pode ser vazio");

        if (description?.Length > 2000)
            throw new DomainException("Descrição não pode ter mais de 2000 caracteres");

        var now = DateTime.UtcNow;

        return new Campaign
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim(),
            System = string.IsNullOrWhiteSpace(system) ? "Call of Cthulhu 7e" : system.Trim(),
            TimePeriod = timePeriod?.Trim(),
            IsPublic = false,
            IsActive = true,
            OwnerId = ownerId,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    /// <summary>
    /// Atualizar detalhes da campanha
    /// </summary>
    public void UpdateDetails(string name, string? description = null, string? timePeriod = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Nome da campanha não pode ser vazio");

        if (name.Length > 200)
            throw new DomainException("Nome da campanha não pode ter mais de 200 caracteres");

        if (description?.Length > 2000)
            throw new DomainException("Descrição não pode ter mais de 2000 caracteres");

        Name = name.Trim();
        Description = description?.Trim();
        TimePeriod = timePeriod?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Definir imagem de capa da campanha
    /// </summary>
    public void SetCoverImage(string? imageUrl)
    {
        if (imageUrl?.Length > 500)
            throw new DomainException("URL da imagem não pode ter mais de 500 caracteres");

        CoverImage = imageUrl?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Tornar a campanha pública (visível para todos)
    /// </summary>
    public void MakePublic()
    {
        IsPublic = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Tornar a campanha privada (visível só para o dono)
    /// </summary>
    public void MakePrivate()
    {
        IsPublic = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Arquivar a campanha (não aparece em listagens)
    /// </summary>
    public void Archive()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Restaurar campanha arquivada
    /// </summary>
    public void Restore()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}