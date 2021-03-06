﻿Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports DevExpress.Data.Filtering
Imports DevExpress.PivotGrid.ServerMode.Queryable
Imports System.Linq.Expressions
Imports DevExpress.Data.Linq

Namespace XPG_ServerMode_EF
    Friend Class DistinctCountFunction
        Implements ICustomFunctionOperator, ICustomFunctionOperatorFormattable, IQueryableConvertible

        Friend Const Name As String = "DistinctCount"

        Public Sub New()
        End Sub

        Private Function ICustomFunctionOperator_Evaluate(ParamArray ByVal operands() As Object) As Object Implements ICustomFunctionOperator.Evaluate
            Return -1
        End Function

        Private ReadOnly Property ICustomFunctionOperator_Name() As String Implements ICustomFunctionOperator.Name
            Get
                Return Name
            End Get
        End Property



        Private Function ICustomFunctionOperator_ResultType(ParamArray ByVal operands() As Type) As Type Implements ICustomFunctionOperator.ResultType
            Return GetType(Integer)
        End Function

        Private Function ICustomFunctionOperatorFormattable_Format(ByVal providerType As Type, ParamArray ByVal operands() As String) As String Implements ICustomFunctionOperatorFormattable.Format
            Return String.Format("Count(distinct {0})", operands(0))
        End Function



        Private Function GetElementType(ByVal thisExpression As Expression) As Type
            Return thisExpression.Type.GetInterfaces().First(Function(type) type.IsGenericType AndAlso type.Name = "IEnumerable`1").GetGenericArguments()(0)
        End Function

        Public Function Convert(ByVal operands As CriteriaOperatorCollection, ByVal thisExpression As ParameterExpression, ByVal converter As ICriteriaToExpressionConverter) As Expression Implements IQueryableConvertible.Convert

            Dim elementType As Type = GetElementType(thisExpression)
            Dim elementParameter As ParameterExpression = Expression.Parameter(elementType, "elem")
            Dim groupSelect As LambdaExpression = Expression.Lambda(converter.Convert(elementParameter, operands(1)), elementParameter)
            Dim groupEpression As Expression = Expression.Call(GetType(Enumerable), "GroupBy", New Type() {elementType, groupSelect.Body.Type}, thisExpression, groupSelect)
            Return Expression.Call(GetType(Enumerable), "Count", New Type() {groupEpression.Type.GetGenericArguments()(0)}, groupEpression)
        End Function

        Public ReadOnly Property QueryableKind() As QueryableKind Implements IQueryableConvertible.QueryableKind

            Get
                Return DevExpress.PivotGrid.ServerMode.Queryable.QueryableKind.Aggregate
            End Get
        End Property

        Public ReadOnly Property ConversionSupported() As Boolean Implements IQueryableConvertible.ConversionSupported
            Get
                Return True
            End Get
        End Property

    End Class
End Namespace
