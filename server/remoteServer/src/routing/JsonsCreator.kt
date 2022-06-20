package com.example.routing

import com.example.game.Game
import com.example.game.Side
import kotlinx.serialization.encodeToString
import kotlinx.serialization.json.Json
import kotlinx.serialization.json.JsonObject
import kotlinx.serialization.json.encodeToJsonElement

fun createGameJSONString(game: Game): String {
    return Json.encodeToString(
        JsonObject(
            mapOf(
                "path" to Json.encodeToJsonElement("game"),
                "body" to JsonObject(
                    mapOf(
                        "state" to Json.encodeToJsonElement(game.state),
                        "score" to Json.encodeToJsonElement(game.score.toString()),
                        "time" to Json.encodeToJsonElement(game.timer.time),
                        "status" to Json.encodeToJsonElement(game.getStatus())
                    )
                )
            )
        )
    )
}

fun createPrepareGameJSONString(game: Game, side: Side): String {
    return Json.encodeToString(
        JsonObject(
            mapOf(
                "path" to Json.encodeToJsonElement("prepareGame"),
                "body" to JsonObject(
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
            )
        )
    )
}