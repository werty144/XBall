package com.xballserver.localserver

import com.example.game.Game
import com.example.game.GameStatus
import com.example.game.Side
import com.xballserver.remoteserver.infrastructure.Lobby
import com.xballserver.remoteserver.infrastructure.UserId
import com.xballserver.remoteserver.routing.APIMove
import com.xballserver.remoteserver.routing.createGameAddresseeJSONString
import com.xballserver.remoteserver.routing.createPrepareGameAddresseeJSONString
import kotlinx.coroutines.*

class GameManager() {
    var gameJob: Job? = null
    var game: Game? = null
    private val updateTime = 15L
    private val gameCoroutineScope: CoroutineScope = CoroutineScope(CoroutineName("Game scope"))

    suspend fun startGameFromLobby(lobby: Lobby) {
        val memberIDs = lobby.members.toList()

        when (lobby.nMembers) {
            1 -> {
                game = Game(0, memberIDs[0], memberIDs[0], lobby.gameProperties, updateTime)
                val firstMemberMessage = createPrepareGameAddresseeJSONString(memberIDs[0], game!!, Side.RIGHT)
                Printer.print(firstMemberMessage)
            }
            2 -> {
                game = Game(0, memberIDs[0], memberIDs[1], lobby.gameProperties, updateTime)
                val firstMemberMessage = createPrepareGameAddresseeJSONString(memberIDs[0], game!!, Side.LEFT)
                val secondMemberMessage = createPrepareGameAddresseeJSONString(memberIDs[1], game!!, Side.RIGHT)
                Printer.print(firstMemberMessage)
                Printer.print(secondMemberMessage)
            }
            else -> {
                return
            }
        }

        game!!.toInitialState()
        gameJob = gameCoroutineScope.launch { runGame(game!!) }
    }

    suspend fun runGame(game: Game) {
        while (true) {
            delay(updateTime)
            game.nextState()
            Printer.print(createGameAddresseeJSONString(game.user1Id, game))
            if (game.user1Id != game.user2Id) {
                Printer.print(createGameAddresseeJSONString(game.user2Id, game))
            }

            if (game.getStatus() == GameStatus.ENDED) {
                stopGame()
            }
        }
    }

    fun makeMove(move: APIMove, actorID: UserId) {
        game?.makeMove(move, actorID)
    }

    fun stopGame() {
        game = null
        gameJob?.cancel()
    }
}