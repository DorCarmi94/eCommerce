using System;
using System.Collections.Generic;
using eCommerce.Common;
using eCommerce.DataLayer;

namespace eCommerce.Business
{
    public class Bid
    {
        private string ID;
        private List<Pair<string, BidState>> ownersApproved;
        private double priceBid;
        private ItemInfo item;
        private int amout;
        private User buyer;
        private int ownersNumber;

        private BidState currentState;
        
        public enum BidState
        {
            WaitingForApprove,
            Approved,
            NotApproved
            
        }

        public string GetID()
        {
            return this.ID;
    }


        public Bid(User buyer,ItemInfo itemInfo, double priceBid, int amount, List<OwnerAppointment> ownerAppointments,User founder)
        {
            currentState = BidState.WaitingForApprove;
            this.item = itemInfo;
            this.priceBid = priceBid;
            this.amout = amount;
            this.buyer = buyer;

            ownersApproved = new List<Pair<string, BidState>>();
                        
            ID = $"item={itemInfo.name};buyer={buyer.Username},date={DateTime.Now}";
            
            ownersApproved.Add(
                new Pair<string, BidState>()
                {
                    HolderId = ID, Key = founder.Username,
                    KeyId = founder.Username, Value = BidState.WaitingForApprove
                });
            ownersNumber++;
            
            foreach (var ownerAppointment in ownerAppointments)
            {
                if (!ownerAppointment.Ownername.Equals(founder.Username))
                {
                    ownersApproved.Add(new Pair<string, BidState>()
                    {
                        HolderId = ID, Key = ownerAppointment.Ownername,
                        KeyId = ownerAppointment.Ownername, Value = BidState.WaitingForApprove
                    });
                    ownersNumber++;
                }
            }

        }


        public Result<BidState> ApproveOrDissapproveBid(string ownerName, bool shouldApprove)
        {
            if (this.currentState == BidState.WaitingForApprove)
            {
                bool found = false;
                foreach (var pair in ownersApproved)
                {
                    if (pair.Key.Equals(ownerName))
                    {
                        if (pair.Value.Equals(BidState.WaitingForApprove))
                        {
                            pair.Value = shouldApprove ? BidState.Approved : BidState.NotApproved;
                            if (shouldApprove == false)
                            {
                                this.currentState = BidState.NotApproved;
                            }
                        }
                        else
                        {
                            return Result.Fail<BidState>("Something went wrong");
                        }
                    }

                    found = true;
                    break;
                }

                if (found)
                {
                    this.ownersNumber--;
                    if (currentState.Equals(BidState.NotApproved))
                    {
                        return Result.Ok(currentState);
                    }
                    else if (ownersNumber == 0)
                    {
                        currentState = BidState.Approved;
                        
                    }
                    return Result.Ok(currentState);
                    
                }
                else
                {
                    return Result.Fail<BidState>("Not an owner to approve this bid");
                }
                
            }
            else
            {
                return Result.Fail<BidState>("Something went wrong");
            }
        }

        public ItemInfo GetItemInfoAfterBidApprove()
        {
            ItemInfo theItem = new ItemInfo(this.item);
            theItem.amount = amout;
            theItem.PricePerUnit = priceBid;
            return theItem;
        }

        public BidInfo GetBidInfo()
        {
            return new BidInfo(this.ID, this.currentState, this.item.ItemName, this.priceBid, this.amout, this.buyer.Username);
        }

        public bool CheckIfShouldApprove(User user)
        {
            bool found = false;
            foreach (var owners in ownersApproved)
            {
                if (owners.Key.Equals(user.Username))
                {
                    found = true;
                    break;
                }
            }

            return found;
        }
    }
    
    public class BidInfo
    {
        public string BidID { get; private set; }
        public Bid.BidState State { get; private set; }
        public string itemID { get; private set; }
        public double price { get; private set; }
        public int amount { get; private set; }
        public string BuyerID { get; private set; }

        public BidInfo(string bidId, Bid.BidState state, string itemID, double price, int amount, string buyerId)
        {
            BidID = bidId;
            State = state;
            this.itemID = itemID;
            this.price = price;
            this.amount = amount;
            BuyerID = buyerId;
        }
    }
}