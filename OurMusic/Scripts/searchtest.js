QUnit.test("empty search", function (assert) {
    document.getElementById("searchQuery").value = "";
    var done = assert.async();
    $("#searchButton").trigger("click");

    setTimeout(function () {
        assert.ok(document.getElementById("vidTitle").value === "", "Passed!");
        assert.ok(document.getElementById("vidUrl").value === "", "Passed!");
        assert.ok(document.getElementById("searchError").innerHTML === "Search queries must be 3-32 characters long!", "Passed!");
        done();
    }, 750);
});

QUnit.test("short search", function (assert) {
    document.getElementById("searchQuery").value = "a";
    var done = assert.async();
    $("#searchButton").trigger("click");

    setTimeout(function () {
        assert.ok(document.getElementById("vidTitle").value === "", "Passed!");
        assert.ok(document.getElementById("vidUrl").value === "", "Passed!");
        assert.ok(document.getElementById("searchError").innerHTML === "Search queries must be 3-32 characters long!", "Passed!");
        done();
    }, 750);
});

QUnit.test("long search", function (assert) {
    document.getElementById("searchQuery").value = "This is a very long search for no real reason other than testing big inputs! wow can you believe that this input is still going? there is no way i'm going to get an actual result for this";
    var done = assert.async();
    $("#searchButton").trigger("click");

    setTimeout(function () {
        assert.ok(document.getElementById("vidTitle").value === "", "Passed!");
        assert.ok(document.getElementById("vidUrl").value === "", "Passed!");
        assert.ok(document.getElementById("searchError").innerHTML === "Search queries must be 3-32 characters long!", "Passed!");
        done();
    }, 750);
});


QUnit.test("random garbage search", function (assert) {
    document.getElementById("searchQuery").value = "ksjdrhgkseh4t4kiy7k";
    var done = assert.async();
    $("#searchButton").trigger("click");

    setTimeout(function () {
        assert.ok(document.getElementById("vidTitle").value === "", "Passed!");
        assert.ok(document.getElementById("vidUrl").value === "", "Passed!");
        assert.ok(document.getElementById("searchError").innerHTML === "No search result found!", "Passed!");
        done();
    }, 750);
});

QUnit.test("valid search", function (assert) {
    document.getElementById("searchQuery").value = "can't killean the zillean";
    var done = assert.async();
    $("#searchButton").trigger("click");

    setTimeout(function () {
        assert.ok(document.getElementById("vidTitle").value === "Can't Killean the Zilean", "Passed!");
        assert.ok(document.getElementById("vidUrl").value === "Xpe-JoGyPsY", "Passed!");
        assert.ok(document.getElementById("searchError").innerHTML === "", "Passed!");
        done();
    }, 750);
});