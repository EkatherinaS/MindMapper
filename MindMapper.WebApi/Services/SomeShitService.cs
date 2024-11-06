using System.Threading.Tasks;
using MindMapper.WebApi.Models;
using MindMapper.WebApi.Services.Interfaces;

namespace MindMapper.WebApi.Services;

public class SomeShitService : ISomeShitService
{
    //Сервисы распознавания и разбора pdf оформляйте аналогично
    public Task<string> SomeAction(SomeCoolModel action)
    {
        return Task.FromResult("Hello World!");
    }
}