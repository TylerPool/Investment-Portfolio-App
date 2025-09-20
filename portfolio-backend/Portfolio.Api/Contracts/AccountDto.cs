using System;
using System.Collections.Generic;
using System.Globalization;
using Portfolio.Api.Models;

namespace Portfolio.Api.Contracts;

public record TradeDto(
    int? Id,
    string? Symbol,
    int? AccountId,
    int Quantity,
    string OpenDate,
    double OpenPrice,
    string? CloseDate,
    double? ClosePrice);

public record AccountDto(
    int Id,
    string? Name,
    decimal? Cash,
    AccountType? AccountType,
    List<TradeDto>? Trades);

public static class AccountDtoExtensions
{
    public static Account ToModel(this AccountDto dto)
    {
        var account = new Account(dto.Id.ToString(CultureInfo.InvariantCulture), dto.AccountType ?? AccountType.TaxableBrokerage)
        {
            Cash = dto.Cash ?? 0
        };

        if (dto.Trades is null)
        {
            return account;
        }

        foreach (var tradeDto in dto.Trades)
        {
            if (string.IsNullOrWhiteSpace(tradeDto.Symbol))
            {
                continue;
            }

            if (!DateOnly.TryParse(tradeDto.OpenDate, out var openDate))
            {
                continue;
            }

            var trade = new Trade(
                tradeDto.Id ?? 0,
                tradeDto.Symbol.Trim(),
                tradeDto.AccountId ?? dto.Id,
                tradeDto.Quantity,
                openDate,
                tradeDto.OpenPrice)
            {
                ClosePrice = tradeDto.ClosePrice
            };

            if (!string.IsNullOrWhiteSpace(tradeDto.CloseDate) && DateOnly.TryParse(tradeDto.CloseDate, out var closeDate))
            {
                trade.CloseDate = closeDate;
            }

            account.Trades.Add(trade);
        }

        return account;
    }
}
