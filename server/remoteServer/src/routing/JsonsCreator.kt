package com.xballserver.remoteserver.routing

import com.xballserver.remoteserver.game.Game
import com.xballserver.remoteserver.game.Side
import com.xballserver.remoteserver.infrastructure.UserId
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import kotlinx.serialization.json.JsonObject
import kotlinx.serialization.json.encodeToJsonElement


fun createGameJSONObject(game: Game): JsonObject {
    return JsonObject(
                mapOf(
                    "state" to Json.encodeToJsonElement(game.state),
                    "score" to Json.encodeToJsonElement(game.score.toString()),
                    "time" to Json.encodeToJsonElement(game.timer.time),
                    "status" to Json.encodeToJsonElement(game.getStatus()),
                )
            )
}

fun createGameJSONString(game: Game): String {
    return Json.encodeToString(
        mapOf(
            "path" to Json.encodeToJsonElement("game"),
            "body" to createGameJSONObject(game)
        )
    )
}

fun createGameAddresseeJSONString(addressee: UserId, game: Game): String {
    return Json.encodeToString(
        mapOf(
            "path" to Json.encodeToJsonElement("game"),
            // need cast to string since no serializer for ULong
            "addressee" to Json.encodeToJsonElement(addressee.toString()),
            "body" to createGameJSONObject(game)
        )
    )
}

fun createPrepareGameJSONObject(game: Game, side: Side): JsonObject {
    return JsonObject(
            mapOf(
                "side" to Json.encodeToJsonElement(side),
                "game" to JsonObject(
                    mapOf(
                        "state" to Json.encodeToJsonElement(game.state),
                        "score" to Json.encodeToJsonElement(game.score.toString()),
                        "time" to Json.encodeToJsonElement(game.timer.time),
                        "status" to Json.encodeToJsonElement(game.getStatus())
                    )
                )
            )
        )
}

fun createPrepareGameJSONString(game: Game, side: Side): String {
    return Json.encodeToString(
        mapOf(
            "path" to Json.encodeToJsonElement("prepareGame"),
            "body" to createPrepareGameJSONObject(game, side)
        )
    )
}

fun createPrepareGameAddresseeJSONString(addressee: UserId, game: Game, side: Side): String {
    return Json.encodeToString(
        mapOf(
            "path" to Json.encodeToJsonElement("prepareGame"),
            // need cast to string since no serializer for ULong
            "addressee" to Json.encodeToJsonElement(addressee.toString()),
            "body" to createPrepareGameJSONObject(game, side)
        )
    )
}

fun createServerReadyJSONString(port: Int): String {
    return Json.encodeToString(
        mapOf(
            "path" to Json.encodeToJsonElement("serverReady"),
            "port" to Json.encodeToJsonElement(port)
        )
    )
}

fun createCancelGameJSONString(): String {
    return Json.encodeToString(
        (
            mapOf(
                "path" to Json.encodeToJsonElement("cancelGame")
            )
        )
    )
}

fun createStartGameJSONString(): String {
    return Json.encodeToString(
        mapOf(
            "path" to Json.encodeToJsonElement("startGame")
        )
    )
}

fun createEndGameJSONString(): String {
    return Json.encodeToString(
        mapOf(
            "path" to Json.encodeToJsonElement("endGame")
        )
    )
}