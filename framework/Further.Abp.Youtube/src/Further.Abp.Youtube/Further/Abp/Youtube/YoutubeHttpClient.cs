using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Options;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Further.Abp.Youtube
{
    public class YoutubeHttpClient : IYoutubeHttpClient, ITransientDependency
    {
        private readonly YouTubeService youtubeService;
        private readonly ILogger<YoutubeHttpClient> logger;

        public YoutubeHttpClient(
            IOptions<YoutubeOptions> options,
            ILogger<YoutubeHttpClient> logger)
        {
            var apiKey = options.Value.ApiKey;
            var applicationName = options.Value.ApplicationName;

            this.youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = applicationName
            });
            this.logger = logger;
        }

        public virtual async Task<VideoContentDetails> GetVideoContentDetails(string videoId)
        {
            return await ExecuteYoutubeApiCall(async () =>
            {
                var videoRequest = youtubeService.Videos.List("contentDetails");
                videoRequest.Id = videoId;

                var videoResponse = await videoRequest.ExecuteAsync();
                return CheckVideoResponse(videoResponse, videoId).ContentDetails;
            }, $"Retrieving content details for video ID: {videoId}");
        }

        public virtual async Task<VideoPlayer> GetVideoPlayer(string videoId)
        {
            return await ExecuteYoutubeApiCall(async () =>
            {
                var videoRequest = youtubeService.Videos.List("player");
                videoRequest.Id = videoId;

                var videoResponse = await videoRequest.ExecuteAsync();
                return CheckVideoResponse(videoResponse, videoId).Player;
            }, $"Retrieving player for video ID: {videoId}");
        }

        public virtual async Task<VideoSnippet> GetVideoSnippet(string videoId)
        {
            return await ExecuteYoutubeApiCall(async () =>
            {
                var videoRequest = youtubeService.Videos.List("snippet");
                videoRequest.Id = videoId;

                var videoResponse = await videoRequest.ExecuteAsync();
                return CheckVideoResponse(videoResponse, videoId).Snippet;
            }, $"Retrieving snippet for video ID: {videoId}");
        }

        public virtual async Task<VideoStatistics> GetVideoStatistics(string videoId)
        {
            return await ExecuteYoutubeApiCall(async () =>
            {
                var videoRequest = youtubeService.Videos.List("statistics");
                videoRequest.Id = videoId;

                var videoResponse = await videoRequest.ExecuteAsync();
                return CheckVideoResponse(videoResponse, videoId).Statistics;
            }, $"Retrieving statistics for video ID: {videoId}");
        }

        public virtual async Task<VideoStatus> GetVideoStatus(string videoId)
        {
            return await ExecuteYoutubeApiCall(async () =>
            {
                var videoRequest = youtubeService.Videos.List("status");
                videoRequest.Id = videoId;

                var videoResponse = await videoRequest.ExecuteAsync();
                return CheckVideoResponse(videoResponse, videoId).Status;
            }, $"Retrieving status for video ID: {videoId}");
        }

        public async Task<List<CommentThread>> GetVideoComments(string videoId,long maxCommentCount = 50)
        {
            return await ExecuteYoutubeApiCall(async () =>
            {
                var request = youtubeService.CommentThreads.List("snippet");
                request.VideoId = videoId;
                request.TextFormat = CommentThreadsResource.ListRequest.TextFormatEnum.PlainText;
                request.MaxResults = maxCommentCount;

                var response = await request.ExecuteAsync();

                return response.Items.ToList();
            }, $"Retrieving comments for video ID: {videoId}");
        }

        public virtual string VideoUrlToId(string videoUrl)
        {
            string videoId = string.Empty;

            if (videoUrl.Contains("youtube.com"))
            {
                var uri = new Uri(videoUrl);
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                videoId = query["v"];
            }
            else if (videoUrl.Contains("youtu.be"))
            {
                var uri = new Uri(videoUrl);
                videoId = uri.Segments[1];
            }

            return videoId;
        }

        protected virtual async Task<T> ExecuteYoutubeApiCall<T>(Func<Task<T>> apiCall, string logMessage)
        {
            try
            {
                if (this.youtubeService.ApiKey.IsNullOrEmpty())
                    throw new Exception("Youtube ApiKey is null or empty.");

                logger.LogInformation(logMessage);
                return await apiCall();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while executing YouTube API call: {logMessage}");
                throw;
            }
        }

        protected virtual Video CheckVideoResponse(VideoListResponse response, string videoId)
        {
            if (response == null || response.Items.Count <= 0)
            {
                throw new Exception($"No video found with ID: {videoId}");
            }

            return response.Items.First(x => x.Id == videoId);
        }
    }
}
