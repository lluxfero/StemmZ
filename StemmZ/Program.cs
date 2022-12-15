//string s = "стремящиеся";
//string s = "полностью";
string s = "ехать";
Console.WriteLine(StemmZ(s));

static string DeleteWordEnding(string word, int lengthReplaceS)
{
    if (word.Length > lengthReplaceS)
    {
        return word[..^lengthReplaceS];
    }
    else { return ""; } // если по ошибке lengthReplaceS больше длины, строка будет пустой, а не ошибочной
}
static string StemmingWord(string word, string sReplaced, string sReplaceWith)
{
    if (word.EndsWith(sReplaced))
    {
        return (DeleteWordEnding(word, sReplaced.Length) + sReplaceWith);
    }
    else { return word; }
}
static string CutWordEnding(string word, string sEnding)
{
    if (word.Length > sEnding.Length)
    {
        return StemmingWord(word, sEnding, "");
    }
    else { return word; }
}

static string ConvertWord(string word) // функция конвертирования токена
{
    // несколько частных замен для унификации обработки разных форм одной лексемы, которые можно убрать

    if (word == "тот") { word = "т"; } // все остальные if-ы обрабатывают конкретные словоформы и могут быть убраны
    if (word == "этот") { word = "эт"; }
    if (word == "мы") { word = "ны"; }
    if (word == "семян") { word = "семен"; }
    if (word == "стремян") { word = "стремен"; }
    if (word == "один") { word = "одн"; }
    if (word == "нас") { word = "нах"; }
    if (word == "вас") { word = "вах"; }
    if (word == "ты") { word = "тоб"; }
    if (word == "много") { word = "многое"; } // иначе распознается как прилагательные вроде "большого"
    if (word == "тебя") { word = "тобя"; }
    if (word == "тебе") { word = "тобе"; }
    if (word == "я") { word = "мнь"; }
    if (word == "он") { word = "й"; }
    if (word == "она") { word = "йа"; }
    if (word == "оно") { word = "йэ"; }
    if (word == "они") { word = "й"; }
    if ((word == "меня") || (word == "зол") || (word == "весь") || (word == "день") || (word == "пень") || (word == "вошь")) // etc.
    
    /* "меня" не содержит беглого гласного и попало случайно, лишь потому, что его надо свести к основе "мн"
    "весь" добавлено, чтобы не произошло разрыва "ве"/"в" при отрывании "сь"
    аналогично "зол", "зо"/"зл" и "л" */

    {
        word = word[0] + word[2..];
    }
    if ((word == "уши") || (word == "ушей") || (word == "ушам") || (word == "ушами") || (word == "ушах"))
    {
        word = word[0] + "х" + word[2..];
    }
    if (word == "двумя") { word = "двуми"; }

    word = word.Replace("и", ",ы");
    word = word.Replace("я", ",а");
    word = word.Replace("ю", ",у");
    word = word.Replace("е", ",э");
    word = word.Replace("ьо", "Ьo"); // замена сочетаний "ьо" на сочетание заглавного мягкого знака с латинской "o", чтобы не исказить слова вроде "каудильо"
    word = word.Replace("ь", ","); // замена строчных мягких знаков на запятые
    word = word.Replace("Ь", "ь"); // замена заглавных мягких знаков на строчные => уничтожаются все мягкие знаки, кроме тех, за которыми следует "о"
    word = word.Replace(",,", ",й");

    if (word[Math.Min(word.Length - 1, 2)..] != "ур,ы")
    {
        word = word.Replace("к,", "к"); // кроме слов "кюри" и "жюри"
        word = word.Replace("ж,", "ж"); // иначе совпали бы с повелительным наклонением ед.числа глаголов "курить" и "журить"
    }
    word = word.Replace("г,", "г");
    word = word.Replace("х,", "х");
    word = word.Replace("ш,", "ш");
    word = word.Replace("ц,", "ц");
    word = word.Replace("ч,", "ч");
    word = word.Replace("щ,", "щ");
    word = word.Replace("ъ,", "ъй");
    word = word.Replace("й,", "йй");
    word = word.Replace("а,", "ай");
    word = word.Replace("о,", "ой");
    word = word.Replace("у,", "уй");
    word = word.Replace("э,", "эй");
    word = word.Replace("ы,", "ый");
    if (word[0] == ',') { word = "й" + word[1..]; } // для слов, начинающихся на йотированный гласный, не отдельные слова, не подлежит убиранию
    word = word.Replace("дву", "два");
    return word;
}

static string FormalStemming(string word) // функция выделения формальной основы
{
    if (word == "йы") { return "и"; } // чтобы союз "и" не объединялся с местоимением третьего лица
    else
    {
        if ((word == "йыл,ы") || (word == "йыл,")) { return "ил,"; }
        else
        {
            word = StemmingWord(word, "кто", "к");
            word = StemmingWord(word, "что", "ч");
            word = CutWordEnding(word, "айас,а"); // предваряет усечение окончаний существительных и прилагательных, которым может предшествовать мягкий "с", чтобы слова "карась" и "карасем" получили одинаковый разбор
            word = CutWordEnding(word, "уйус,а");
            word = CutWordEnding(word, "ым,ыс,а");
            word = CutWordEnding(word, "м,ы");
            if (word.Length > 4)
            {
                word = CutWordEnding(word, "йу"); // сохраняются формы вроде "дую", "дуя", "мою", "моя"
                word = CutWordEnding(word, "йа");
            }
            word = CutWordEnding(word, "ах");
            word = CutWordEnding(word, "ых");
            word = CutWordEnding(word, "а");
            word = CutWordEnding(word, "у");
            word = CutWordEnding(word, "ы");
            word = CutWordEnding(word, "э");
            word = CutWordEnding(word, "ый");
            word = CutWordEnding(word, "эй");
            word = CutWordEnding(word, "ой");
            if (word.Length > 3)
            {
                word = CutWordEnding(word, "ом");
                word = CutWordEnding(word, "ым");
            }
            word = CutWordEnding(word, "ого");
            word = CutWordEnding(word, "эго");
            word = CutWordEnding(word, "эв");
            word = CutWordEnding(word, "ов");
            if (word.Length > 3)
            {
                word = CutWordEnding(word, "ам");
                word = CutWordEnding(word, "эм");
            }
            word = CutWordEnding(word, "с,"); // усекаются слова с возвратным суффиксом, но также и слова типа "карась"
            word = StemmingWord(word, "шэл", "шл"); // после деёфикации "шол" -> "шл" не нужно
            word = CutWordEnding(word, "ого");
            word = CutWordEnding(word, "эго");
            word = CutWordEnding(word, "о");
            word = CutWordEnding(word, "а");
            word = CutWordEnding(word, "ы");
            if (word.Length > 2) { word = CutWordEnding(word, "л"); } // ограничение на длину, чтобы "злой" объединялось со "злость"
            if (word.Length > 3) { word = CutWordEnding(word, "л,"); } // не существует слов с последовательностью "льл", а, например, с "лле" есть
            word = CutWordEnding(word, "ы");
            word = CutWordEnding(word, "э");
            word = CutWordEnding(word, "о");
            word = CutWordEnding(word, "у");
            word = CutWordEnding(word, "а");
            word = CutWordEnding(word, "от,");
            word = CutWordEnding(word, "эт,");
            word = CutWordEnding(word, "ыт,");
            word = CutWordEnding(word, "ат,");
            word = CutWordEnding(word, "ут,");
            word = CutWordEnding(word, "ый");
            if (word.Length > 3)
            {
                word = CutWordEnding(word, "ом");
                word = CutWordEnding(word, "ам"); // для случаев вроде "имамам"
                word = CutWordEnding(word, "эм");
                word = CutWordEnding(word, "ым");
                word = CutWordEnding(word, "эт");
                word = CutWordEnding(word, "ыт");
                word = CutWordEnding(word, "ут");
                word = CutWordEnding(word, "эш");
                word = CutWordEnding(word, "ош");
                word = CutWordEnding(word, "уй");
                word = CutWordEnding(word, "ай");
                word = CutWordEnding(word, "ой");
                word = CutWordEnding(word, "эй");
                word = CutWordEnding(word, "от"); // чтобы спасти причастия на "от" от различения кратких и части полных форм
                word = CutWordEnding(word, "ущ");
                word = CutWordEnding(word, "ащ");
                word = CutWordEnding(word, "ат");
            }
            word = StemmingWord(word, "шэдш", "ш"); // не "шл", потому что в приставочных у "шл" "л" отрывается;
            word = CutWordEnding(word, "вш");
            word = CutWordEnding(word, "эств");
            word = CutWordEnding(word, "ств");
            word = CutWordEnding(word, "в"); // собирает не только деепричастия, но и глаголы вроде "жить", "плыть", "слыть"
            word = CutWordEnding(word, "о");
            word = CutWordEnding(word, "э");
            word = CutWordEnding(word, "й");
            word = StemmingWord(word, "дш", "д");
            word = StemmingWord(word, "тш", "т");
            word = StemmingWord(word, "бш", "б");
            word = StemmingWord(word, "пш", "п");
            word = StemmingWord(word, "гш", "г");
            word = StemmingWord(word, "кш", "к");
            word = StemmingWord(word, "сш", "с");
            word = StemmingWord(word, "нн", "н");
            word = StemmingWord(word, "н,эн", "н");
            word = StemmingWord(word, "вл,эч", "вл,эк");
            word = StemmingWord(word, "л,эч", "л,эг");
            word = StemmingWord(word, "стр,ыч", "стр,ыг");
            word = StemmingWord(word, "моч", "мог");
            word = StemmingWord(word, "б,эр,эч", "б,эр,эг");
            word = StemmingWord(word, "ст,эр,эч", "ст,эр,эг");
            if (word.Length > 5)
            {
                word = StemmingWord(word, "ст,ыч", "ст,ыг"); // помещая сюда, спасаем имя собственное "Стич", бесприставочного глагола *стичь не существует
            }
            word = StemmingWord(word, "м,эн", "м,");
            word = StemmingWord(word, "жэск", "г");
            word = CutWordEnding(word, "эск");
            word = CutWordEnding(word, "ск");
            word = StemmingWord(word, "л,эц", "л,к");
            word = StemmingWord(word, "ч", "к");
            word = StemmingWord(word, ",эц", "к");
            word = StemmingWord(word, "ц", "к");
            if (word.Length > 3)
            {
                word = StemmingWord(word, "эк", "к");
                word = StemmingWord(word, "ок", "к");
                if (word.Length > 4)
                {
                    word = StemmingWord(word, "рш", "р"); // помещая сюда, спасаем слово "ёрш"
                    if (word.Length > 5)
                    {
                        word = StemmingWord(word, "ст,эн", "стн"); // помещая сюда, спасаем слово "стена"
                        word = CutWordEnding(word, "ост,");
                        word = CutWordEnding(word, "остн");
                        if (word.Length > 6)
                        {
                            word = CutWordEnding(word, "эст,");
                            word = CutWordEnding(word, "эстн");
                            if (word.Length > 7)
                            {
                                word = CutWordEnding(word, "ыт,эл,");
                                word = CutWordEnding(word, "т,эл,");
                                if (word.Length > 8) //"котельный"
                                {
                                    word = CutWordEnding(word, "ыт,эл,н");
                                    word = CutWordEnding(word, "т,эл,н");
                                }
                            }
                        }
                    }
                }
            }
            word = CutWordEnding(word, "т,"); // так поздно, чтобы собирались всякие "ость" и "есть"; большинство инфинитивов съедятся с гласными раньше
            word = StemmingWord(word, "мн,", "м");
            word = StemmingWord(word, "мн", "м");
            word = StemmingWord(word, "шл", "ш"); // чтобы собрать "шедший" и "шёл"
            word = CutWordEnding(word, ",");
            word = CutWordEnding(word, ",н");
            word = StemmingWord(word, "дам", "д");
            word = StemmingWord(word, "даш", "д");
            word = StemmingWord(word, "даст", "д");
            word = StemmingWord(word, "дад", "д");
            word = StemmingWord(word, "йэст", "йэд");
            word = StemmingWord(word, "йэш", "йэд");
            word = StemmingWord(word, "йэм", "йэд");
            return word;
        }
    }
}

static string BackConvertWord(string word) // функция обратного конвертирования токена
{
    word = word.Replace(",ы", "и");
    word = word.Replace(",э", "е");
    word = word.Replace(",а", "я");
    word = word.Replace(",у", "ю");
    word = word.Replace("йы", "и");
    word = word.Replace("йэ", "е");
    word = word.Replace("йа", "я");
    word = word.Replace("йу", "ю");
    word = word.Replace(",", "ь");
    word = word.Replace("жы", "жи");
    word = word.Replace("шы", "ши");
    word = word.Replace("чы", "чи");
    word = word.Replace("цы", "ци");
    word = word.Replace("щы", "щи");
    word = word.Replace("кы", "ки");
    word = word.Replace("хы", "хи");
    word = word.Replace("гы", "ги");
    word = word.Replace("цэ", "це");
    word = word.Replace("жэ", "же");
    word = word.Replace("шэ", "ше");
    word = word.Replace("чэ", "че");
    word = word.Replace("щэ", "ще");
    word = word.Replace("кэ", "ке");
    word = word.Replace("хэ", "хе");
    word = word.Replace("гэ", "ге");
    word = word.Replace("ьo", "ьо"); // восстанавливаем русскую "о" в словах типа "каудильо"
    return word;
}

string StemmZ(string word)
{
    s = ConvertWord(s);
    s = FormalStemming(s);
    s = BackConvertWord(s);
    return s;
}