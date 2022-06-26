package com.xballserver.localserver

import io.ktor.websocket.*


class Printer {
    companion object {
        var session: DefaultWebSocketSession? = null

        suspend fun print(message: String) {
            session?.send(message)
        }
    }
}