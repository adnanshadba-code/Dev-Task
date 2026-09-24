

using DevTeam.Services.Identity.Application.Commands.Users;
using DevTeam.Services.Identity.Application.DTOs;
using DevTeam.Services.Identity.Domain.Entities;

public static class UserMapping
{
    // DTO → Command
    //recieve from client

    //Api Contract from user , 
    public static CreateUserCommand ToCommand(
       this CreateUserDto dto)
    {
        return new CreateUserCommand
        {
            Username = dto.Username,
            Password = dto.Pswword,
            CreateBy = dto.CreateBy,
            PermissionIds = dto.PermissionIds
        };
    }

    // Command → Entity
    //Use case ما العملية التي أريد تنفيذها؟
    public static User ToEntity(
          this CreateUserCommand command)
    {
        return new User
        {
            Username = command.Username,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreateBy = command.CreateBy
        };
    }

    // Entity → DTO//
    //return to client 
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username
        };
    }


    // DTO → Command
    public static UpdateUserCommand ToCommand(
       this UpdateUserDto dto,
       int id)
    {
        return new UpdateUserCommand
        {
            Id = id,
            Username = dto.Username,
            Password = dto.Password,
            UpdateBy = dto.UpdateBy
        };
    }


    // Command → Existing Entity
    // UpdateUserCommand -> User

    public static User UpdateFrom(
        this UpdateUserCommand command,
        User user)
    {
        user.Username = command.Username;
        user.UpdateBy = command.UpdateBy;
        user.UpdatedAt = DateTime.UtcNow;

        return user;
    }


}