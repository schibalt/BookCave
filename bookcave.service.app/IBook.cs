using BookCave.Service.Dto;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace BookCave
{
    [ServiceContract(Namespace = "urn:BookCave.Service", Name="BookCaveService")]
    public interface IBook
    {
        [OperationContract]
        [WebInvoke(Method = "POST",
            //BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            //RequestFormat = WebMessageFormat.Json,
            UriTemplate = "books")]
        ResultDto PostLexileData(LexileDto book);

        [OperationContract]
        [WebInvoke(Method = "POST",
            //BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            //RequestFormat = WebMessageFormat.Json,
            UriTemplate = "books/bn")]
        ResultDto PostBarnesData(BarnesDto bnData);

        [OperationContract]
        [WebInvoke(Method = "POST",
            //BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            //RequestFormat = WebMessageFormat.Json,
            UriTemplate = "books/scholastic")]
        ResultDto PostScholasticData( ScholasticDto scholasticData);

        //[OperationContract]
        //[WebInvoke(
        //Method = "POST",
        //    BodyStyle = WebMessageBodyStyle.Bare,
        //    ResponseFormat = WebMessageFormat.Json,
        //    RequestFormat = WebMessageFormat.Json,
        //    UriTemplate = "NewBooks")]
        //BookResponseDto PostBooks(List<BookDto> books);

        [OperationContract]
        [WebGet( BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json,
            RequestFormat = WebMessageFormat.Json,
            UriTemplate = "books/{isbn13}")]
        LexileDto GetBook(string isbn13);

        [OperationContract]
        [WebGet( 
            //BodyStyle = WebMessageBodyStyle.Bare,
            //ResponseFormat = WebMessageFormat.Json,
            //RequestFormat = WebMessageFormat.Json,
            UriTemplate = "/books?age={ageVal}&title={titleVal}&lex={lexVal}&author={authorVal}&summary={summVal}")]
        List<LexileDto> GetBooks(string ageVal, string titleVal, string lexVal, string authorVal,string summVal);
    }
}