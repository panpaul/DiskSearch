using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace DiskSearch.Worker.Services
{
    public class SearchService : Search.SearchBase
    {
        private readonly ILogger<SearchService> _logger;

        public SearchService(ILogger<SearchService> logger)
        {
            _logger = logger;
        }

        public override Task<SearchReply> DoSearch(SearchRequest request, ServerCallContext context)
        {
            _logger.LogDebug($"DoSearch: Word:{request.Word} | Tag:{request.Tag}");
            var results = Worker.Backend.Search(request.Word, request.Tag);
            var convert = results.Select(item => new Scheme
            {
                Path = item.Path,
                Description = item.Description
            }).ToList();
            return Task.FromResult(new SearchReply {Results = {convert}});
        }

        public override Task<CommandReply> Control(CommandRequest request, ServerCallContext context)
        {
            _logger.LogDebug($"Update: Cmd:{request.Cmd}");
            switch (request.Cmd)
            {
                case CommandRequest.Types.Command.ReloadBlackList:
                    Worker.Backend.UpdateBlackList();
                    break;
                case CommandRequest.Types.Command.ReloadPath:
                    Worker.Backend.DeleteAll();
                    Worker.Backend.UnWatch();
                    Worker.Backend.Watch(request.Path);
                    Task.Run(() => { Worker.Backend.Walk(request.Path); });
                    break;
                default:
                    return Task.FromResult(new CommandReply {Status = false});
            }

            return Task.FromResult(new CommandReply {Status = true});
        }
    }
}