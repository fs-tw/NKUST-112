using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Further.Abp.Youtube
{
    public interface IYoutubeHttpClient
    {
        //統計資料與觀看
        Task<VideoStatistics> GetVideoStatistics(string videoId);

        //影片基本資訊
        Task<VideoSnippet> GetVideoSnippet(string videoId);

        //影片詳細資訊
        Task<VideoContentDetails> GetVideoContentDetails(string videoId);

        //影片狀態
        Task<VideoStatus> GetVideoStatus(string videoId);

        //嵌入回應式影片
        Task<VideoPlayer> GetVideoPlayer(string videoId);

        //影片頂層留言
        Task<List<CommentThread>> GetVideoComments(string videoId, long maxCommentCount = 50);

        string VideoUrlToId(string videoUrl);
    }
}
