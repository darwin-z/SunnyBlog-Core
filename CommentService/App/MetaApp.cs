﻿using CommentService.App.Interface;
using CommentService.Response;
using Google.Protobuf.WellKnownTypes;
using Infrastructure;
using Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using static ArticleService.Rpc.Protos.gArticle;

namespace CommentService.App
{
    public class MetaApp : IMetaApp
    {
        private readonly IDbContextFactory<CommentDBContext> contextFactory;
        private readonly gArticleClient articleRpc;
        public MetaApp(IDbContextFactory<CommentDBContext> contextFactory, gArticleClient articleRpc)
        {
            this.contextFactory = contextFactory;
            this.articleRpc = articleRpc;
        }

        /// <summary>
        /// 获取文章元数据
        /// </summary>
        /// <param name="aid"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Meta GetMeta(int aid,int? uid = null)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                var sql = @$"
                select 
                    a.id as ArticleId,
                    (select count(*) from [Views] v where v.articleId = a.Id) as ViewCount,
                    (select count(*) from Likes l where l.articleId = a.Id and (l.status = 1 or l.status = 3)) as LikeCount,
                    (select count(*) from Likes l where l.articleId = a.id and (l.status = 2 or l.status = 3)) as CollectionCount,
                    (select count(*) from Comments c where c.articleId = a.id) as CommentCount,
                    ({(uid.HasValue ? $"select count(*) from Likes l where l.articleId = a.Id and (l.status = 1 or l.status = 3) and l.userId = {uid.Value}" : "select 0")}) as IsUserLike,
                    ({(uid.HasValue ? $"select count(*) from Likes l where l.articleId = a.Id and (l.status = 2 or l.status = 3) and l.userId = {uid.Value}" : "select 0")}) as IsUserCollection
                from 
                article as a 
                where a.id = {aid}
                group by a.id
                ";
                var metaList = dbContext.Database.SqlQuery<Meta>(sql);

                return metaList.FirstOrDefault();
            }
        }

        /// <summary>
        /// 获取一批文章互动元数据
        /// </summary>
        /// <param name="aids"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<Meta> GetMetaList(int[] aids,int?uid = null)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //ef太慢了
                /*var commentList = await dbContent.Comments.ToListAsync();
                var likeList = await dbContent.Likes.ToListAsync();
                var viewList = await dbContent.Views.ToListAsync();
                var metas = from a in aids
                            join c in commentList on a equals c.ArticleId into comment
                            join l in likeList on a equals l.ArticleId into like
                            join v in viewList on a equals v.ArticleId into view
                            group new Meta {
                                ArticleId = a,
                                ViewCount = view.Count(vv => vv.ArticleId == a),
                                LikeCount = like.Count(ll => ll.ArticleId == a && ll.Status == 1),
                                CommentCount = comment.Count(cc => cc.ArticleId == a),
                                CollectionCount = like.Count(cc => cc.ArticleId == a && cc.Status == 2),
                                IsUserLike = uid.HasValue && like.FirstOrDefault(ll => ll.ArticleId == a && ll.UserId == uid.Value && ll.Status == 1) != null ? 1 : 0,
                                IsUserCollection = uid.HasValue && like.FirstOrDefault(ll => ll.ArticleId == a && ll.UserId == uid.Value && ll.Status == 2) != null ? 1 : 0,
                            } by a;

                var metaList = new List<Meta>();
                foreach (var meta in metas)
                {
                    foreach (var m in meta)
                    {
                        metaList.Add(m);
                    }
                }*/
                if(aids.Length > 0)
                {
                    var sql = @$"
                    select 
                        a.id as ArticleId,
                        (select count(*) from [Views] v where v.articleId = a.Id) as ViewCount,
                        (select count(*) from Likes l where l.articleId = a.Id and (l.status = 1 or l.status = 3)) as LikeCount,
                        (select count(*) from Likes l where l.articleId = a.id and (l.status = 2 or l.status = 3)) as CollectionCount,
                        (select count(*) from Comments c where c.articleId = a.id) as CommentCount,
                        ({(uid.HasValue ? $"select count(*) from Likes l where l.articleId = a.Id and (l.status = 1 or l.status = 3) and l.userId = {uid.Value}" : "select 0")}) as IsUserLike,
                        ({(uid.HasValue ? $"select count(*) from Likes l where l.articleId = a.Id and (l.status = 2 or l.status = 3) and l.userId = {uid.Value}" : "select 0")}) as IsUserCollection
                    from 
                    article as a 
                    where a.id in({string.Join(',', aids)})
                    group by a.id
                    ";
                    var metaList = dbContext.Database.SqlQuery<Meta>(sql);

                    return metaList.ToList();
                }
                return new List<Meta>();
            }
        }
    
        /// <summary>
        /// 获取用户的互动数据
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public async Task<UserMeta> GetUserMeta(int uid)
        {
            using (var dbContext = contextFactory.CreateDbContext())
            {
                //查询用户的文章
                var aids = (await articleRpc.GetArticleListAsync(new Empty())).Infos.Where(a => a.UserId == uid).Select(a => a.Id).ToArray();
                var articleMetas = GetMetaList(aids);
                var userMeta = new UserMeta
                {
                    UserId = uid,
                    ViewCount = articleMetas.Sum(m => m.ViewCount),
                    LikeCount = articleMetas.Sum(m => m.LikeCount),
                    CommentCount = articleMetas.Sum(m => m.CommentCount),
                    CollectionCount = articleMetas.Sum(m => m.CollectionCount)
                };

                return userMeta;
            }
        }
    }
}
