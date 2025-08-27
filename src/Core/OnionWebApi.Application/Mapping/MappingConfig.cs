namespace OnionWebApi.Application.Mapping;
public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Brand, GetBrandQueryResponse>()
            .Map(dest => dest.NameId, src => $"{src.Name}-{src.Id}")
            .Map(dest => dest.CreatedUserName, src => src.CreatedUser.UserName)
            .Map(dest => dest.UpdatedUserName, src => src.UpdatedUser.UserName)
            .Map(dest => dest.DeletedUserName, src => src.DeletedUser.UserName);
    }
}
