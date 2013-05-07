using BookCave.Service.Dto;
using BookCave.Service.Entities;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace BookCave
{
    [ServiceContract(Namespace = "urn:BookCave.Service", Name = "BookCaveService")]
    public interface IBook
    {
        [OperationContract]
        [WebInvoke
        (
            Method = "POST",
            //BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            //RequestFormat = WebMessageFormat.Json,
            UriTemplate = "lexile"
        )]
        ResultDto PostLexileData(LexileDto book);

        [OperationContract]
        [WebInvoke
        (
            Method = "POST",
            //BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            //RequestFormat = WebMessageFormat.Json,
            UriTemplate = "barnes"
        )]
        ResultDto PostBarnesData(BarnesDto bnData);

        [OperationContract]
        [WebInvoke
        (
            Method = "POST",
            //BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            //RequestFormat = WebMessageFormat.Json,
            UriTemplate = "scholastic"
        )]
        ResultDto PostScholasticData(ScholasticDto scholasticData);

        [OperationContract]
        [WebInvoke
        (
            Method = "POST",
            ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "commonsense"
        )]
        ResultDto PostCommonSenseData(CommonSenseDto scholasticData);

        //[OperationContract]
        //[WebInvoke(
        //Method = "POST",
        //    BodyStyle = WebMessageBodyStyle.Bare,
        //    ResponseFormat = WebMessageFormat.Json,
        //    RequestFormat = WebMessageFormat.Json,
        //    UriTemplate = "NewBooks")]
        //BookResponseDto PostBooks(List<BookDto> books);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "skill/{isbn13}")]
        SkillDto GetSkillMetrics(string isbn13);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "content/{isbn13}")]
        ContentDto GetContentMetrics(string isbn13);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "books/{isbn13}")]
        SuperDto GetBookData(string isbn13);

        [OperationContract]
        [WebGet(
            //BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            //RequestFormat = WebMessageFormat.Json,
            UriTemplate = "books?content={content}&title={title}&skill={skill}&author={author}&summary={summary}")]
        List<SuperDto> GetBooks(string content, string title, string skill, string author, string summary);
    }
}