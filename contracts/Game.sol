pragma solidity ^0.4.17;

contract Game {

    address public owner;

    uint256 counter = 0; //what, just do length bro

    struct Session {
        uint256 id;
        address[] slots;
        uint256 playersN;
        uint256 winnerN; 
        uint betValue;
        SessionState currentState;
    }
    //massive addressov
    Session[] sessions;     //something like this, need to be controlled for overflow

    //mapping of struct and var numStructs
    enum SessionState {noGame, gameStarted, gameClosed}

    modifier ownerOnly {
        if (msg.sender != owner) 
        revert();
        _;
    }

    function Game() public {
        owner = msg.sender;
    }

    function InitializeSession(uint256 plN,/*players array*/ uint betV) ownerOnly public {
        Session session;
        session.id = counter + 1;
        session.playersN = plN;
        session.betValue = betV;
        session.currentState = SessionState.noGame;
        sessions.push(session);
    }

    function EnterSession(uint256 id) payable public {
        require(msg.value == sessions[id].betValue);
        require(sessions[id].slots.length < sessions[id].playersN);
        sessions[id].slots.push(msg.sender);
        //Check if they entering through the game and not scamming (how?)
    }

    function withdraw(uint256 sessionId, address winner, uint reward) ownerOnly public {
        //Pay the winner his winnings
        require(winner == sessions[sessionId].slots[sessions[sessionId].winnerN]);  //oh looks nice
        require(sessions[sessionId].currentState == SessionState.gameStarted);
        sessions[sessionId].currentState == SessionState.gameClosed;
        winner.transfer(reward);
    }

    //function refund half if player disconnected before the start

    function () {
        revert();
    }
}