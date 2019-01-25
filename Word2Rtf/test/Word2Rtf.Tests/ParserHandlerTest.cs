using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using Word2Rtf.Parsers;
using Word2Rtf.Models;

namespace Word2Rtf.Tests
{
    public class ParserHandlerTests
    {
        [Fact]
        public void Test_BracketParser_EnglishFirst_WithMultipleBibleVerses()
        {
            var input = new [] 
            {
                "【宣告/Proclaim】《詩篇/Psalm 50:23》;《希伯來書/Hebrew 13:15b》"
            };
            var element = ParserHandler.Parse(input).FirstOrDefault();
            Assert.NotNull(element);
            Assert.Equal(input.First(), element.Input);
            Assert.True(element.Pass);
            Assert.Equal(2, element.Verses.Count());
            Assert.Equal(ElementType.Title, element.ElementType);
            Assert.Equal("Proclaim\nPsalm 50:23\nHebrew 13:15b", element.Verses.First().Content);
            Assert.Equal(Language.English, element.Verses.First().Language);
            Assert.Equal("宣告\n詩篇 50:23\n希伯來書 13:15b", element.Verses.Last().Content);
            Assert.Equal(Language.Chinese, element.Verses.Last().Language);            
        }

        [Fact]
        public void Test_BracketParser_BibleVerses()
        {
            var title = "【Responsive Reading啟應讀經】詩篇 Psalm 29";
            var chinese = "神的眾子阿，你們要將榮耀能力，歸給耶和華，歸給耶和華。2. 要將耶和華的名所當得的榮耀歸給他，以聖潔的妝飾敬拜耶和華。3. 耶和華的聲音發在水上，榮耀的神打雷，耶和華打雷在大水之上。4. 耶和華的聲音大有能力，耶和華的聲音滿有威嚴。5. 耶和華的聲音震破香柏樹，耶和華震碎利巴嫩的香柏樹。6. 他也使之跳躍如牛犢，使利巴嫩和西連跳躍如野牛犢。7. 耶和華的聲音使火焰分岔。8. 耶和華的聲音震動曠野，耶和華震動加低斯的曠野。9. 耶和華的聲音驚動母鹿落胎，樹木也脫落淨光。凡在他殿中的，都稱說他的榮耀。10. 洪水泛濫之時，耶和華坐著為王，耶和華坐著為王，直到永遠。11\"耶和華必賜力量給他的百姓，耶和華必賜平安的福給他的百姓。\"";
            var english = "1Ascribe to the LORD , O mighty ones, ascribe to the LORD glory and strength. 2Ascribe to the LORD the glory due his name; worship the LORD in the splendor of his holiness.3The voice of the LORD is over the waters; the God of glory thunders, the LORD thunders over the mighty waters.4The voice of the LORD is powerful; the voice of the LORD is majestic.5	The voice of the LORD breaks the cedars; the LORD breaks in pieces the cedars of Lebanon.6He makes Lebanon skip like a calf, Sirion like a young wild ox.7	The voice of the LORD strikes with flashes of lightning.8The voice of the LORD shakes the desert; the LORD shakes the Desert of Kadesh.9The voice of the LORD twists the oaks and strips the forests bare. And in his temple all cry, \"Glory!\"10The LORD sits enthroned over the flood; the LORD is enthroned as King forever.11The LORD gives strength to his people; the LORD blesses his people with peace.";
            var input = new [] { title, chinese, english };

            var elements = ParserHandler.Parse(input).ToArray();
            Assert.NotNull(elements);
            Assert.Equal(3, elements.Count());

            Assert.True(elements[0].Pass);
            Assert.Equal(title, elements[0].Input);
            Assert.Equal(2, elements[0].Verses.Count());
            Assert.Equal("Responsive Reading\nPsalm 29", elements[0].Verses.First().Content);
            Assert.Equal("啟應讀經\n詩篇 29", elements[0].Verses.Skip(1).First().Content);

            Assert.True(elements[1].Pass);
            Assert.Equal(chinese, elements[1].Input);
            Assert.Equal(1, elements[1].Verses.Count());
            Assert.Equal(chinese, elements[1].Verses.First().Content);

            Assert.True(elements[2].Pass);
            Assert.Equal(english, elements[2].Input);
            Assert.Equal(1, elements[2].Verses.Count());
            Assert.Equal(english, elements[2].Verses.First().Content);

        } 
    }
}