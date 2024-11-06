using System.Threading.Tasks;
using MindMapper.WebApi.Models;

namespace MindMapper.WebApi.Services.Interfaces;

public interface ISomeShitService
{
    public Task<string> SomeAction(SomeCoolModel action);
}