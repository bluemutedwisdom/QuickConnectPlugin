﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Windows.Forms;
using KeePassLib;

namespace QuickConnectPlugin.PasswordChanger {

    [Serializable]
    public class PasswordChangerTreeNode : TreeNode, IPasswordChangerTreeNode {

        private PwGroup pwGroup;
        private PwDatabase pwDatabase;
        private IFieldMapper fieldMapper;

        protected PasswordChangerTreeNode(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context) {
        }

        private PasswordChangerTreeNode(PwGroup pwGroup, PwDatabase pwDatabase, IFieldMapper fieldMapper)
            : base(pwGroup.Name) {
            this.pwGroup = pwGroup;
            this.pwDatabase = pwDatabase;
            this.fieldMapper = fieldMapper;
        }

        public static PasswordChangerTreeNode Build(PwDatabase pwDatabase, IFieldMapper fieldMapper) {
            PasswordChangerTreeNode rootTreeNode = new PasswordChangerTreeNode(pwDatabase.RootGroup, pwDatabase, fieldMapper);
            build(rootTreeNode, pwDatabase.RootGroup, pwDatabase, fieldMapper);
            return rootTreeNode;
        }

        private static void build(PasswordChangerTreeNode parentTreeNode, PwGroup rootGroup, PwDatabase pwDatabase, IFieldMapper fieldMapper) {
            foreach (var group in rootGroup.Groups) {
                PasswordChangerTreeNode treeNode = new PasswordChangerTreeNode(group, pwDatabase, fieldMapper);
                parentTreeNode.Nodes.Add(treeNode);
                build(treeNode, group, pwDatabase, fieldMapper);
            }
        }

        public TreeNode Root {
            get { return this; }
        }

        public ICollection<IPasswordChangerHostPwEntry> GetEntries() {
            return this.GetEntries(false);
        }

        public ICollection<IPasswordChangerHostPwEntry> GetEntries(bool includeSubGroupEntries) {
            var entries = new Collection<IPasswordChangerHostPwEntry>();
            foreach (var pwEntry in this.pwGroup.GetEntries(includeSubGroupEntries)) {
                entries.Add(new PasswordChangerHostPwEntry(pwEntry, this.pwDatabase, this.fieldMapper));
            }
            return entries;
        }
    }
}