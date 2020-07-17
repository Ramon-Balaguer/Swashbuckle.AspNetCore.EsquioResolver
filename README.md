# Swashbuckle.AspNetCore.EsquioResolver
Conflict solver in actions with features toggles using Esquio.

When using feature toggles you will probably want to use the same endpoint / resource / verb depending on whether a feature toggles is enabled.

```csharp
[HttpGet]
[ActionName("Get")]
public IEnumerable<Pokemon> Get()
{
	return pokemonReader.Read(configuration.GetValue<string>("PokedexPath"));
}

[FeatureFilter(Name = Flags.PokedexSuper)]
[HttpGet]
[ActionName("Get")]
public IEnumerable<PokemonSuper> GetNew()
{
	return pokemonSuperReader.Read(configuration.GetValue<string>("PokedexSuperPath"));
}
```

The Swashbuckle documentation does not allow two endpoints / resource / verb to be called the same, because this solution is not supported by OpenApi 3.0.

The solution is to create a conflict solver.

# Requirements
- .Net Core 3.1.302
- Nuget: Esquio 
- Nuget: Swashbuckle 

# Installation
With package manager:
```
Install-Package Swashbuckle.AspNetCore.EsquioResolver
```

.Net CLI
```
dotnet add Swashbuckle.AspNetCore.EsquioResolver
```

# How to use
Add these commands in your project's startup.cs ConfigureServices method:
```csharp
services.AddSwaggerGen(configuration =>
{
	configuration.ResolveConflictingActionsByFeatureToggles();
});
``` 

And in your Configure method:
```csharp
app.UseResolveConflictingActionsByFeatureToggles();
```
