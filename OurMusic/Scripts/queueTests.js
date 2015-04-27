QUnit.test("refreshList", function (assert) {
    var testJSON = "[{\"$id\":\"1\",\"title\":\"Video One\",\"url\":\"Url One\",\"votes\":1},{\"$id\":\"2\",\"title\":\"Video Two\",\"url\":\"Url Two\",\"votes\":1},{\"$id\":\"3\",\"title\":\"Video Three\",\"url\":\"Url Three\",\"votes\":1}]";
    refreshListTest(testJSON);

    var rowOne = document.getElementById("queueList").rows.namedItem("Url One");
    var rowTwo = document.getElementById("queueList").rows.namedItem("Url Two");
    var rowThree = document.getElementById("queueList").rows.namedItem("Url Three");
    assert.ok(rowOne.rowIndex == 1, "Passed!");
    assert.ok(rowTwo.rowIndex == 2, "Passed!");
    assert.ok(rowThree.rowIndex == 3, "Passed!");
    assert.ok(document.getElementById("queueList").rows.length == 4, "Passed!");
});
/*
QUnit.test("short search", function (assert) {


    var testJSON = "[{\"$id\":\"1\",\"title\":\"Video One\",\"url\":\"Url One\",\"votes\":1},{\"$id\":\"2\",\"title\":\"Video Two\",\"url\":\"Url Two\",\"votes\":1},{\"$id\":\"3\",\"title\":\"Video Three\",\"url\":\"Url Three\",\"votes\":1}]";
    window.setTimeout(function () { }, 5000);
    refreshListTest(testJSON);

    var rowOne = document.getElementById("queueList").rows.namedItem("Url One");
    var rowTwo = document.getElementById("queueList").rows.namedItem("Url Two");
    var rowThree = document.getElementById("queueList").rows.namedItem("Url Three");

    var buttonsArrayOne = rowOne.getElementsByTagName("BUTTON");
    var upvoteBtnOne = buttonsArrayOne[0];
    var downvoteBtnOne = buttonsArrayOne[1];

    var buttonsArrayTwo = rowTwo.getElementsByTagName("BUTTON");
    var upvoteBtnTwo = buttonsArrayTwo[0];
    var downvoteBtnTwo = buttonsArrayTwo[1];

    var buttonsArrayThree = rowThree.getElementsByTagName("BUTTON");
    var upvoteBtnThree = buttonsArrayThree[0];
    var downvoteBtnThree = buttonsArrayThree[1];

    var done = assert.async();
    upvoteBtnThree.click();
    
    setTimeout(function () {
        assert.ok(rowOne.rowIndex == 2, "Passed!");
        assert.ok(rowTwo.rowIndex == 3, "Passed!");
        assert.ok(rowThree.rowIndex == 1, "Passed!");
        done();
    }, 200000);


});
*/
QUnit.test("refreshList", function (assert) {
    var testJSON = "[{\"$id\":\"1\",\"title\":\"Video One\",\"url\":\"Url One\",\"votes\":1},{\"$id\":\"2\",\"title\":\"Video Two\",\"url\":\"Url Two\",\"votes\":1},{\"$id\":\"3\",\"title\":\"Video Three\",\"url\":\"Url Three\",\"votes\":1}]";
    refreshListTest(testJSON);

    var rowOne = document.getElementById("queueList").rows.namedItem("Url One");
    var rowTwo = document.getElementById("queueList").rows.namedItem("Url Two");
    var rowThree = document.getElementById("queueList").rows.namedItem("Url Three");
    assert.ok(rowOne.rowIndex == 1, "Passed!");
    assert.ok(rowTwo.rowIndex == 2, "Passed!");
    assert.ok(rowThree.rowIndex == 3, "Passed!");
    assert.ok(document.getElementById("queueList").rows.length == 4, "Passed!");

    adjustVotesAndPlacementTest("Url One", -1, -2);
    assert.ok(rowOne.rowIndex == 3, "Passed!");
    assert.ok(rowTwo.rowIndex == 1, "Passed!");
    assert.ok(rowThree.rowIndex == 2, "Passed!");

    adjustVotesAndPlacementTest("Url Three", 1, 1);
    assert.ok(rowOne.rowIndex == 3, "Passed!");
    assert.ok(rowTwo.rowIndex == 2, "Passed!");
    assert.ok(rowThree.rowIndex == 1, "Passed!");


});
