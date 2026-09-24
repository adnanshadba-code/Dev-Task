using DevTeam.Services.Identity.Application.Commands.Permissions;
using DevTeam.Services.Identity.Application.DTOs;
using DevTeam.Services.Identity.Domain.Entities;

namespace DevTeam.Services.Identity.Application.Mappings;

public static class PermissionMapping
{

    // CreatePermissionDto -> Command
    // =========================================

    public static CreatePermissionCommand ToCommand(this CreatePermissionDto dto)
    {
        return new CreatePermissionCommand
        {
            Name = dto.Name,
            Description = dto.Description
        };
    }

    // =========================================
    // UpdatePermissionDto -> Command
    public static UpdatePermissionCommand ToCommand(this UpdatePermissionDto dto,int id)
    {
        return new UpdatePermissionCommand
        {
            Id = id,
            Name = dto.Name,
            Description = dto.Description
        };
    }

    // CreateCommand -> Entity
    // =========================================

    public static Permission ToEntity(this CreatePermissionCommand command)
    {
        return new Permission
        {
            Name = command.Name,
            Description = command.Description,
            CreatedAt = DateTime.UtcNow
        };
    }

    // UpdateCommand -> Existing Entity
    // =========================================

    public static Permission UpdateFrom(this UpdatePermissionCommand command,Permission permission)
    {
        permission.Name = command.Name;
        permission.Description = command.Description;
        permission.UpdatedAt = DateTime.UtcNow;

        return permission;
    }

    // Entity -> DTO
    // =========================================

    public static PermissionDto ToDto(this Permission permission)
    {
        return new PermissionDto
        {
            Id = permission.Id,
            Name = permission.Name,
            Description = permission.Description
        };
    }
}