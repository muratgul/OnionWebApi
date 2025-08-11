namespace OnionWebApi.Application.Mapping;
public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Brand, GetBrandQueryResponse>()
            .Map(dest => dest.NameId, src => $"{src.Name} {src.Id}");
    }
}
