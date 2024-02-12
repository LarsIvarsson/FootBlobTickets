
const fixturesContainer = document.getElementById("fixturesList");

fetch('https://app-gladpack-projekt.azurewebsites.net/api/Tickets')
    .then(res => res.json())
    .then(data => {
        const fixturesHTML = generateFixtureHTML(data);
        fixturesContainer.innerHTML = fixturesHTML;
    });

function generateFixtureHTML(fixtures) {
    return fixtures.map(f => `<li id="${f.FixtureId}">${f.HomeTeam} - ${f.AwayTeam} || Available tickets:  ${f.StadiumCapacity - f.TicketsSold} <input type="radio" value="${f.FixtureId}" name="fixture" /></li>`).join('');
};



document.getElementById("postButton").addEventListener("click", postTickets);



// const url = "https://app-gladpack-projekt.azurewebsites.net/api/Tickets?numberOfTickets=2";
// const body = " hämta från radio btn ";

const postTickets = async () => {
    await fetch(url, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(body),
    });
};