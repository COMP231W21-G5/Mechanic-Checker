ALTER TABLE Product ADD CONSTRAINT FKProduct508972 FOREIGN KEY (SellerId) REFERENCES Seller (SellerId) ON DELETE Cascade;
ALTER TABLE Address ADD CONSTRAINT FKAddress290155 FOREIGN KEY (SellerId) REFERENCES Seller (SellerId) ON DELETE Cascade;
