pragma solidity ^0.4.17;

contract Game {

    address public owner;

    uint256 counter = 0; //what, just do length bro

    struct Session {
        uint256 id;
        address[] slots;
        uint256 playersN;
        uint256 winnerN; //may be needless
        uint betValue;
        SessionState currentState;
    }
    //massive addressov
    Session[] sessions;     //something like this, both need to be controlled for overflow
    mapping(uint256 => Session) sessionsM;  //do i need this

    //mapping of struct and var numStructs
    enum SessionState {noGame, gameStarted, gameEnded}

    modifier ownerOnly {
        if (msg.sender != owner) 
        revert();
        _;
    }

    function Game() public {
        owner = msg.sender;
    }

    function InitializeSession(uint256 plN,/*players massive*/ uint betV) ownerOnly public {
        Session session;
        session.id = counter + 1;
        session.playersN = plN;
        session.betValue = betV;
        session.currentState = SessionState.noGame;
        sessions.push(session);
    }

    function EnterSession(uint256 id) payable public {
        require(msg.value == sessionsM[id].betValue);
        require(sessionsM[id].slots.length < sessionsM[id].playersN);
        sessionsM[id].slots.push(msg.sender);
        //Check if they entering through the game and not scamming (how?)
    }

    function withdraw() ownerOnly public {
        //Pay the winner his winnings
    }

    //function refund half if player disconnected before the start

    function () {
        revert();
    }
}