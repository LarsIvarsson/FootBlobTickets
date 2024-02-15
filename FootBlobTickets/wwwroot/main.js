const fixturesContainer = document.getElementById("fixturesList");
const selectedMatches = document.getElementById("matchSelected");
var email = document.getElementById("email");
var selectElement = document.getElementById("numberSelect");
const postButton = document.getElementById("postButton");
let selectedFixtureId;

postButton.addEventListener("click", (event) => postTickets(event));

const postTickets = async (event) => {
    event.preventDefault();
    const url = "https://app-gladpack-projekt.azurewebsites.net/api/Tickets";

    if (ValidateEmail(email.value)) {
        const body = {
            fixtureId: selectedFixtureId,
            numberOfTickets: selectElement.value,
            email: email.value,
        };

        await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(body),
        });
    }
};

fetch("https://app-gladpack-projekt.azurewebsites.net/api/Tickets", {
    method: "GET",
    mode: "cors",
    headers: {
        "Content-Type": "application/json",
    },
})
    .then((res) => res.json())
    .then((data) => {
        const fixturesHTML = generateFixtureHTML(data);
        fixturesContainer.innerHTML = fixturesHTML;
    });

function generateFixtureHTML(fixtures) {
    fixturesContainer.addEventListener("click", function (e) {
        const homeTeam = e.target.getAttribute("data-home-team");
        const awayTeam = e.target.getAttribute("data-away-team");
        if (homeTeam && awayTeam)
            displayMatch(`Match selected: ${homeTeam} vs ${awayTeam} `);
        else {
            displayMatch(`No matches selected`);
        }

        const selectedCard = document.querySelector(".selected-card");
        if (selectedCard) {
            selectedCard.classList.remove("selected-card");
        }

        e.target.closest(".match-column").classList.add("selected-card");

        selectedFixtureId = e.target.getAttribute("value");
    });

    return fixtures
        .map(
            (f) =>
                `
        <div class="match-column" name="fixture" value="${f.FixtureId
                }" data-home-team="${f.HomeTeam}" data-away-team="${f.AwayTeam}">
          <div class="card-container-row" name="fixture" name="fixture" value="${f.FixtureId
                }" data-home-team="${f.HomeTeam}" data-away-team="${f.AwayTeam}">
            <h2 class="match-name" name="fixture" value="${f.FixtureId
                }" data-home-team="${f.HomeTeam}" data-away-team="${f.AwayTeam}">${f.HomeTeam
                } vs ${f.AwayTeam}</h2>
            <h4 class="match-stadium" name="fixture" value="${f.FixtureId
                }" data-home-team="${f.HomeTeam}" data-away-team="${f.AwayTeam}">${f.Stadium
                }</h4>
            <h6 name="fixture" value="${f.FixtureId}" data-home-team="${f.HomeTeam
                }" data-away-team="${f.AwayTeam
                }">Available Tickets: <em name="fixture" value="${f.FixtureId
                }" data-home-team="${f.HomeTeam}" data-away-team="${f.AwayTeam}">${f.StadiumCapacity - f.TicketsSold
                }</em></h6>
            </div>
        </div>
      `
        )
        .join("");
}

function displayMatch(info) {
    selectedMatches.textContent = info;
}

populateSelect();
function populateSelect() {
    for (var i = 1; i <= 10; i++) {
        var option = document.createElement("option");
        option.value = i;
        option.textContent = i;
        selectElement.appendChild(option);
    }
}

function ValidateEmail(input) {
    var validRegex =
        /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$/;

    if (input.match(validRegex)) {
        return true;
    } else {
        alert("Invalid email address!");
        email.focus();
        return false;
    }
}