using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.DiscountsAndPurchases.Purchases.Rules.CombineRules;
using eCommerce.Business.DiscountsAndPurchases.Purchases.RulesInfo;
using eCommerce.Business.Purchases;

using eCommerce.Common;

namespace eCommerce.Business.DiscountPoliciesCombination
{
    public class Xor : CombinatedComposite
    {
        private Composite _A;
        private Composite _B;
        private Compare _comperator;
        private FieldToCompare _fieldToCompare;
        private ItemInfo _itemToCompare;

        public Xor(Composite a, Composite b, Compare comperator, FieldToCompare fieldToCompare, ItemInfo itemToCompare)
        {
            _A = a;
            _B = b;
            _comperator = comperator;
            _fieldToCompare = fieldToCompare;
            _itemToCompare = itemToCompare;
        }
        public Xor(Composite a, Composite b, Compare comperator, FieldToCompare fieldToCompare)
        {
            _A = a;
            _B = b;
            _comperator = comperator;
            _fieldToCompare = fieldToCompare;
            _itemToCompare = null;
        }

        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, User checkItem2)
        {
            var checkA = _A.Check(checkItem1, checkItem2);
            var checkB = _B.Check(checkItem1, checkItem2);
            Composite theChosenOne;
            if (checkA.Count > 0 && checkB.Count > 0)
            {
                var theChosenOneRes = HandleFieldToCompare(checkItem1, checkItem2);
                if (theChosenOneRes.IsSuccess)
                {
                    theChosenOne = theChosenOneRes.Value;
                }
                else
                {
                    return new Dictionary<string, ItemInfo>();
                }
                    
            }
            else if(checkA.Count>0)
            {
                theChosenOne = _A;
            }
            else if(checkB.Count>0)
            {
                theChosenOne = _B;
            }
            else
            {
                return new Dictionary<string, ItemInfo>();
            }

            return theChosenOne.Check(checkItem1,checkItem2);
        }

        public override bool CheckOneItem(ItemInfo itemInfo, User checkItem2)
        {
            throw new NotImplementedException();
        }

        public override bool CheckIfDiscount()
        {
            return _A.CheckIfDiscount() && _B.CheckIfDiscount();
        }

        public override Result<double> Get(IBasket basket, User user)
        {
            if (CheckIfDiscount())
            {
                double price = basket.GetRegularTotalPrice();
                var checkA = _A.Check(basket, user);
                var checkB = _B.Check(basket, user);
                Composite theChosenOne;
                if (checkA.Count > 0 && checkB.Count > 0)
                {
                    var theChosenOneRes = HandleFieldToCompare(basket, user);
                    if (theChosenOneRes.IsSuccess)
                    {
                        theChosenOne = theChosenOneRes.Value;
                    }
                    else
                    {
                        return Result.Fail<double>(theChosenOneRes.Error);
                    }
                    
                }
                else if(checkA.Count>0)
                {
                    theChosenOne = _A;
                }
                else if(checkB.Count>0)
                {
                    theChosenOne = _B;
                }
                else
                {
                    return Result.Ok(price);
                }
                var chosenPrice=theChosenOne.Get(basket, user);
                if (chosenPrice.IsSuccess)
                {
                    var diff = chosenPrice.Value / price;
                    price = price * diff;
                        
                }
                else
                {
                    Result.Fail("Problem getting the result");
                }
                return Result.Ok(price);
            }
            else
            {
                return Result.Fail<double>("Not a discount");
            }
        }

        public override Result<double> GetOneItem(ItemInfo itemInfo, User user)
        {
            throw new NotImplementedException();
        }

        private Result<Composite> HandleFieldToCompare(IBasket basket, User user)
        {   var getA = _A.Get(basket, user);
            var getB = _B.Get(basket, user);
            if (getA.IsFailure)
            {
                return Result.Fail<Composite>(getA.Error);
            }
            if(getB.IsFailure)
            {
                return Result.Fail<Composite>(getB.Error);
            }
            
            switch (_fieldToCompare)
            {
                case FieldToCompare.TotalPrice:
                    var resultTotalPrice = _comperator.GetResult(getA.Value, getB.Value);
                    if (resultTotalPrice.Equals(getA.Value))
                    {
                        return Result.Ok(_A);
                    }
                    else
                    {
                        return Result.Ok(_B);
                    }
                break;
                case FieldToCompare.NumberOfItems:
                    int totalAmountA = _A.Check(basket, user).Count;
                    int totalAmountB = _B.Check(basket, user).Count;
                    var resultNumberOfItems = _comperator.GetResult(totalAmountA, totalAmountB);
                    if (resultNumberOfItems.Equals(totalAmountA))
                    {
                        return Result.Ok(_A);
                    }
                    else
                    {
                        return Result.Ok(_B);
                    }
                break;
                case FieldToCompare.SpecificItem:
                    if (_itemToCompare == null)
                    {
                        return Result.Fail<Composite>("No item to compare");
                    }
                    var lstA = _A.Check(basket, user);
                    var lstB = _B.Check(basket, user);
                    if (lstA.Where(x=>x.Value.name.Equals(_itemToCompare.name)).ToList().Count>0)
                    {
                        return Result.Ok(_A);
                    }
                    else
                    {
                        return Result.Ok(_B);
                    }
                break;
                case FieldToCompare.SpecificItemPrice:
                    if (_itemToCompare == null)
                    {
                        return Result.Fail<Composite>("No item to compare");
                    }
                    var priceItemA = _A.GetOneItem(this._itemToCompare, user);
                    var priceItemB = _B.GetOneItem(this._itemToCompare, user);
                    var comparedPrice = _comperator.GetResult(priceItemA.Value, priceItemB.Value);
                    if (comparedPrice.Equals(priceItemA))
                    {
                        return Result.Ok(_A);
                    }
                    else
                    {
                        return Result.Ok(_B);
                    }
                break;
                default:
                    var resultTotalPriceDefault = _comperator.GetResult(getA.Value, getB.Value);
                    if (resultTotalPriceDefault.Equals(getA.Value))
                    {
                        return Result.Ok(_A);
                    }
                    else
                    {
                        return Result.Ok(_B);
                    }
            }
        }


        public override Result<RuleInfoNode> GetRuleInfo_A()
        {
            return _A.GetRuleInfo();
        }

        public override Result<RuleInfoNode> GetRuleInfo_B()
        {
            return _B.GetRuleInfo();
        }

        public override Result<DiscountInfoNode> GetDiscountInfo_A()
        {
            return _A.GetDisocuntInfo();
        }

        public override Result<DiscountInfoNode> GetDiscountInfo_B()
        {
            return _B.GetDisocuntInfo();
        }

        public override Combinations GetCombination()
        {
            return Combinations.XOR;
        }

        public override Result<DiscountInfoNode> GetDisocuntInfo()
        {
            var resA = _A.GetDisocuntInfo();
            var resB = _B.GetDisocuntInfo();
            if (resA.IsFailure)
            {
                return Result.Fail<DiscountInfoNode>(resA.Error);
            }
            if (resB.IsFailure)
            {
                return Result.Fail<DiscountInfoNode>(resB.Error);
            }
            return Result.Ok<DiscountInfoNode>(new DiscountInfoCompositeNode(_A.GetDisocuntInfo().Value, _B.GetDisocuntInfo().Value,
                this._comperator.GetComperatorInfo(), _fieldToCompare, _itemToCompare));
        }

        public override Result<RuleInfoNode> GetRuleInfo()
        {
            var resA = _A.GetRuleInfo();
            var resB = _B.GetRuleInfo();
            if (resA.IsFailure)
            {
                return Result.Fail<RuleInfoNode>(resA.Error);
            }
            if (resB.IsFailure)
            {
                return Result.Fail<RuleInfoNode>(resB.Error);
            }

            return Result.Ok<RuleInfoNode>(new RuleInfoNodeComposite(_A.GetRuleInfo().Value, _B.GetRuleInfo().Value,
                _comperator.GetComperatorInfo(),
                _fieldToCompare, _itemToCompare));
        }
    }
}