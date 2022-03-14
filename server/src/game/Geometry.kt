package com.example.game

import kotlinx.serialization.Serializable
import java.util.*
import kotlin.math.*

const val EPS: Double = 1e-5

@Serializable
data class Point(val x: Double, val y: Double) {
    operator fun plus(vector: Vector): Point {
        return Point(x + vector.x, y + vector.y)
    }

    fun approximatelyEqual(other: Point): Boolean {
        return (abs(x - other.x) < EPS) and (abs(y - other.y) < EPS)
    }
}

fun distance(p1: Point, p2: Point) = Vector(p1, p2).length()

@Serializable
class Vector{
    val x: Double
    val y: Double

    constructor(x: Double, y: Double) {
        this.x =  x
        this.y = y
    }

    constructor(p1: Point, p2: Point) {
        val xDiff = p2.x - p1.x
        val yDiff = p2.y - p1.y
        this.x = xDiff
        this.y = yDiff
    }

    override fun toString(): String {
        return "Vector($x, $y)"
    }

    fun unit(): Vector {
        val length = length()
        if (length == 0.0) throw IllegalStateException("Zero vector")
        return Vector(x / length, y / length)
    }

    operator fun times(n: Double): Vector = Vector(x * n, y * n)

    operator fun plus(other: Vector): Vector = Vector(x + other.x, y + other.y)

    fun length(): Double = sqrt(x * x + y * y)

    fun angle(): Double = atan2(y, x)

    fun rotated(angle: Double): Vector = Vector(x * cos(angle) - y * sin(angle), x * sin(angle) + y * cos(angle))

    fun orientedAngleWithVector(v: Vector): Double = atan2(x * v.y - y * v.x, x * v.x + y * v.y)

    fun angleWithVector(v: Vector): Double = abs(orientedAngleWithVector(v))

    fun orthogonalUnit(): Vector = Vector(-y, x).unit()

    fun reflect(other: Vector): Vector = this.rotated(this.orientedAngleWithVector(other) * 2)
}

fun viewAngle(player: Player, point: Point): Double {
    if (point == player.state.position) return 0.0
    return player.state.orientation.angleWithVector(Vector(player.state.position, point))
}

class Line {
    val a: Double
    val b: Double
    val c: Double

    constructor(a: Double, b: Double, c: Double) {
        if ((a == 0.0) and (b == 0.0)) throw IllegalArgumentException("Bad line parameters")
        this.a = a
        this.b = b
        this.c = c
    }

    constructor(p1: Point, p2: Point) {
        if (p1 == p2) throw IllegalArgumentException("Can't construct line from two identical points")
        this.a = p1.y - p2.y
        this.b = p2.x - p1.x
        this.c = p1.x*p2.y - p2.x*p1.y
    }

    fun containsPoint(p: Point): Boolean {
        return abs(a*p.x + b*p.y + c) < EPS
    }

    override fun toString(): String {
        return "Line(a=$a, b=$b, c=$c)"
    }
}

fun intersectionPoint(line1: Line, line2: Line): Point? {
    val det = line1.a * line2.b - line1.b * line2.a
    if (abs(det) < EPS) return null
    val detX = (-line1.c) * line2.b - line1.b * (-line2.c)
    val detY = line1.a * (-line2.c) - (-line1.c) * line2.a
    return Point(detX/det, detY/det)
}


data class Segment(val p1: Point, val p2: Point) {
    init {
        if (p1 == p2) throw IllegalArgumentException("Can't construct segment from two identical points")
    }
    val line = Line(p1, p2)

    fun approximatelyEqual(other: Segment) = p1.approximatelyEqual(other.p1) and p2.approximatelyEqual(other.p2)

    fun containsPoint(p: Point): Boolean {
        if (!line.containsPoint(p)) return false
        return (p.x >= min(p1.x, p2.x) - EPS) and (p.x <= max(p1.x, p2.x) + EPS) and (p.y >= min(p1.y, p2.y) - EPS) and (p.y <= max(p1.y, p2.y) + EPS)
    }

    fun getReflectionTail(other: Segment): Segment? {
        val intersectionPoint = intersectionPoint(this, other) ?: return null
        if (intersectionPoint.approximatelyEqual(p2) or intersectionPoint.approximatelyEqual(p1)) return null
        val tailVector = Vector(intersectionPoint, p2)
        val otherSegmentVector = Vector(other.p1, other.p2)
        val reflectedTailVector = tailVector.reflect(otherSegmentVector)
        val reflectedEndPoint = intersectionPoint + reflectedTailVector
        return Segment(intersectionPoint, reflectedEndPoint)
    }

    fun getReflectionSequence(gameProperties: GameProperties): LinkedList<Point> {
        val breakPoints = LinkedList<Point>()
        var reflectionsFound = true
        var reflectionTail = this
        while (reflectionsFound) {
            // make sure start point is not on ball boundaries
            reflectionTail = Segment(
                gameProperties.clipToBoundaries(reflectionTail.p1, gameProperties.ballRadius + 10*EPS),
                reflectionTail.p2
            )
            // need several reflections in the case ball hits corner (corner case )))00)0) )
            // in that case reflections are summed as vectors
            val reflections = gameProperties.ballBoundaries.mapNotNull { reflectionTail.getReflectionTail(it) }

             if (reflections.isEmpty()) {
                 reflectionsFound = false
                 breakPoints.add(reflectionTail.p2)
            } else {
                val reflectionStartPoint = reflections[0].p1
                if (!reflections.all { it.p1.approximatelyEqual(reflectionStartPoint) }) {
                    throw IllegalStateException("Intersected in different points")
                }
                val resultReflectionVector = reflections.map { Vector(it.p1, it.p2) }.fold(Vector(0.0, 0.0)) { v1: Vector, v2: Vector -> v1 + v2 }
                val reflectionEndPoint = reflectionStartPoint + resultReflectionVector
                breakPoints.add(reflectionStartPoint)
                reflectionTail = Segment(reflectionStartPoint, reflectionEndPoint)
            }
        }
        return breakPoints
    }
}

fun intersectionPoint(segment1: Segment, segment2: Segment): Point? {
    val lineIntersectionPoint = intersectionPoint(segment1.line, segment2.line) ?: return null
    return if (segment1.containsPoint(lineIntersectionPoint) and segment2.containsPoint(lineIntersectionPoint)) {
        lineIntersectionPoint
    } else {
        null
    }
}

