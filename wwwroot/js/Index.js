document.getElementById('URLs').querySelector('.title').onclick = function () {
    document.getElementById('URLs').classList.toggle('open');
};


function GridClick() {
    let elements = document.getElementById("newsID").children;
    for (let i = 0; i < elements.length; i++) {
        let elementsNested = elements[i].children;
        for (let j = 0; j < elementsNested.length; j++) {
            if (elementsNested[j].name == "articleFeed") {
                elementsNested[j].classList.add('day-news-feed');
                break;
            }
        }
    }
    let news = document.getElementById("newsID");
    news.name = "grid";
};

function LineClick() {
    let elements = document.getElementById("newsID").children;
    for (let i = 0; i < elements.length; i++) {
        let elementsNested = elements[i].children;
        for (let j = 0; j < elementsNested.length; j++) {
            if (elementsNested[j].name == "articleFeed") {
                elementsNested[j].classList.remove('day-news-feed');
                break;
            }
        }
    }
    let news = document.getElementById("newsID");
    news.name = "line";
};

async function RSSDataControl() {
    const response = await fetch("/api/ClassMain/RssUrlsControl", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    if (response.ok == true) {
        const sc = await response.json();
        return sc;
    }
}
async function RSSTimerControl() {
    const response = await fetch("/api/ClassMain/RSSTimerControl", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    if (response.ok == true) {
        const sc = await response.json();
        return sc;
    }
}

async function AddFeedControl(url) {
    const response = await fetch("/api/ClassMain/AddFeedControl", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify(url)
    });
    if (response.ok == true) {
        const sc = await response.json();
        return sc;
    }
}
async function DelFeedControl(urls) {
    const response = await fetch("/api/ClassMain/DelFeedControl", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify(urls)
    });
    if (response.ok == true) {
        const sc = await response.json();
        return sc;
    }
}
async function ChangeStatusControl(urls) {
    const response = await fetch("/api/ClassMain/ChangeStatusControl", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify(urls)
    });
    if (response.ok == true) {
        const sc = await response.json();
        return sc;
    }
}


async function CreatRSSFeeds() {
    const rssData = await RSSDataControl();
    let activeFeeds = document.getElementById("ActiveFeedsID");
    rssData.forEach(feed => {
        CreatRSSFeed(feed);
        activeFeeds.append(CreatRSSFeed(feed));
        if (feed.status == "activate") {
            CreatDayNews(feed);
        }
    });
    var timer = Number(RSSTimerControl());
    //await setInterval(() => location.reload(), timer);
}
function CreatDayNews(feed) {
    let news = document.getElementById("newsID");

    const articleFeed = document.createElement("article");
    const articleFeednested = document.createElement("article");
    const titleFeed = document.createElement("h2");
    titleFeed.textContent = feed.title;
    articleFeed.name = "articleFeed";
    articleFeednested.append(titleFeed);
    for (let i = 0; i < feed.items.length; i++) {
        const article = document.createElement("article");
        article.className = "day-news";
        const title = document.createElement("h3");
        title.style = "font-weight: normal;";
        const a = document.createElement("a");
        a.href = feed.items[i].link;
        a.target = "_blank";
        a.innerHTML = feed.items[i].title;
        title.append(a);    
        article.innerHTML = feed.items[i].description;
        article.insertBefore(title, article.firstChild);
        articleFeed.append(article);
        articleFeednested.append(articleFeed);      
    }
    news.append(articleFeednested);
}
function CreatRSSFeed(feed) {
    const input = document.createElement("input");
    input.type = "checkbox";
    const li = document.createElement("li");
    li.id = feed.rssUrl;
    const a = document.createElement("a");
    a.href = feed.link;
    a.target = "_blank";
    if (feed.status == "activate") {
        a.innerHTML = feed.title + " 🔓";
    }
    if (feed.status == "deactivate") {
        a.innerHTML = feed.title + " 🔒";
    }
    li.append(input);
    li.append(a);
    return li;
}

async function AddFeed() {
    let news = document.getElementById("newsID");
    let maket = news.name;

    let url = document.getElementById("searchID").value;
    const feed = await AddFeedControl([url]);
    
    if (feed.title != "No channel") {
        let activeFeeds = document.getElementById("ActiveFeedsID");
        activeFeeds.append(CreatRSSFeed(feed));
        await CreatDayNews(feed);

        if (maket == "grid") {
            GridClick();
        }
    }
    
}
async function DelFeed() {
    if (!confirm("Удалить эти каналы?")) { return;}
    let activeFeedsChidren = document.getElementById("ActiveFeedsID").children;
    let urls = [];
    for (let i = 0; i < activeFeedsChidren.length; i++) {
        if (activeFeedsChidren[i].children[0].checked) {
            urls.push(activeFeedsChidren[i].id);
        }
    }
    await DelFeedControl(urls);
    let news = document.getElementById("newsID");
    let maket = news.name;
    let activeFeeds = document.getElementById("ActiveFeedsID");
    activeFeeds.textContent = '';
    news.textContent = '';
    await CreatRSSFeeds();

    if (maket == "grid") {
        GridClick();
    }
}
async function StopStartFeed() {
    let activeFeedsChidren = document.getElementById("ActiveFeedsID").children;
    let urls = [];
    for (let i = 0; i < activeFeedsChidren.length; i++) {
        if (activeFeedsChidren[i].children[0].checked) {
            urls.push(activeFeedsChidren[i].id);
        }
    }
    await ChangeStatusControl(urls);
    let news = document.getElementById("newsID");
    let maket = news.name;
    let activeFeeds = document.getElementById("ActiveFeedsID");
    activeFeeds.textContent = '';
    news.textContent = '';
    await CreatRSSFeeds();

    if (maket == "grid") {
        GridClick();
    }
}