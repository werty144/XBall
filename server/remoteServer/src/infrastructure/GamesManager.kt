package com.xballserver.remoteserver.infrastructure

import com.xballserver.remoteserver.game.Game
import com.xballserver.remoteserver.game.GameId
import com.xballserver.remoteserver.game.GameStatus
import com.xballserver.remoteserver.routing.APIMove
import com.xballserver.remoteserver.routing.createEndGameJSONString
import com.xballserver.remoteserver.routing.createGameJSONString
import kotlinx.coroutines.*
import java.util.Collections

class GamesManager(val connectionManager: ConnectionManager) {
    private var spareGameId: GameId = 0
    val games: MutableSet<Game> = Collections.synchronizedSet(LinkedHashSet())
    private val updateTime = 15L
    private val gameCoroutineScope: CoroutineScope = CoroutineScope(CoroutineName("Game scope"))
    val runningGames: MutableSet<Pair<GameId, Job>> = Collections.synchronizedSet(LinkedHashSet())

    fun gameById(gameId: GameId) = games.find {it.gameId == gameId}

    fun getGameForUser(userId: UserId): Game? = games.find { (it.user1Id == userId) or (it.user2Id == userId) }

    fun userHasGames(userId: UserId): Boolean = games.any { (it.user1Id == userId) or (it.user2Id == userId) }

    fun makeMove(gameId: GameId, move: APIMove, actorId: UserId) {
        val game = gameById(gameId)
        game?.makeMove(move, actorId)
    }

    fun createGameFromLobby(lobby: Lobby): Game? {
        val userIds = lobby.members.toList()
        val game = when (lobby.maxCapacity) {
            1 -> {
                Game(spareGameId++, userIds[0], userIds[0], lobby.gameProperties, updateTime)
            }
            2 -> {
                Game(spareGameId++, userIds[0], userIds[1], lobby.gameProperties, updateTime)
            }
            else -> {
                null
            }
        } ?: return null
        games.add(game)
        return game
    }
    fun startGame(game: Game) {
        game.toInitialState()
        val gameJob = gameCoroutineScope.launch { runGame(game) }
        runningGames.add(Pair(game.gameId, gameJob))
    }

    suspend fun runGame(game: Game) {
        while (true) {
            delay(updateTime)
            game.nextState()
            val message = createGameJSONString(game)

            connectionManager.sendMessage(game.user1Id, message)
            connectionManager.sendMessage(game.user2Id, message)

            if (game.getStatus() == GameStatus.ENDED) {
                endGame(game.gameId)
                return
            }
        }
    }

    suspend fun endGame(gameId: GameId) {
        val game = games.find { it.gameId == gameId } ?: return

        runningGames.find { it.first == gameId }?.second?.cancel()
        runningGames.removeIf { it.first == gameId }

        val message = createEndGameJSONString()

        connectionManager.sendMessage(game.user1Id, message)
        connectionManager.sendMessage(game.user2Id, message)

        games.removeIf { it.gameId == gameId }
    }

    suspend fun stopAll() {
        games.forEach { endGame(it.gameId) }
    }



    fun clean() {

    }
}
