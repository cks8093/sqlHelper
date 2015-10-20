using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{



    public class User 
    {
        public int Id { get; set; }
        public string USERID { get; set; }
        public string USER_PSWD { get; set; }
        public string LASTNAME { get; set; }
        public string FIRSTNAME { get; set; }
        public string EMAIL { get; set; }
        public DateTime PSWD_EXPIRE { get; set; }
    }

    public class User_test
    {
        public int num { get; set; }
        public string col1 { get; set; }
    }

    public class test
    {
        public string purtype = "";

        public string purtype1 = "초기값";

        public DateTime expireDate;

        public int price = 0;

    }




    #region 재생시작
    public class playStartModel
    {
        public string jumpingPoint = "";
        public List<contentUrlList> contentUrlList;
        public List<thumbnailUrlList> thumbnailUrlList;
    }

    public class contentUrlList
    {
        public string fileSize = "";
        public string playTime = "";
        public string isHD = "";
        public string otu = "";
        public string isDRM = "";
    }

    public class thumbnailUrlList
    {
        public string imageUrl = "";
    } 
    #endregion

    public class playEndModel
    {
        public string nextContentID = "";
        public string buyYN = "";
        public string episodeNo = "";
        public string title = "";
        public string contentPrice = "";
        public string jumpingPoint = "0";
    }

    public class buyCheckModel
    {
        public string contentType = "N";
        public string expireDate = "";
        public string price = "";
    }
}
